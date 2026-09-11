using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static int Main(string[] args)
    {
        string inPath = args[0];
        string outPath = args[1];

        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(inPath)));
        resolver.AddSearchDirectory(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "refs"));
        var readerParams = new ReaderParameters { ReadWrite = false, AssemblyResolver = resolver };
        var asm = AssemblyDefinition.ReadAssembly(inPath, readerParams);
        var module = asm.MainModule;

        if (args.Length > 2 && args[2] == "--scan")
        {
            foreach (var t in module.Types)
            {
                foreach (var m in t.Methods)
                {
                    foreach (var p in m.Parameters)
                    {
                        if (p.HasConstant && p.Constant != null)
                        {
                            var ns = p.ParameterType?.Namespace ?? "";
                            if (ns.Contains("Ports") || ns.Contains("IO"))
                            {
                                Console.WriteLine(t.FullName + "::" + m.Name + "(" + p.Name + " : " + p.ParameterType.FullName + " = " + p.Constant + ")");
                            }
                        }
                    }
                }
            }
            return 0;
        }

        var usbMonitorType = module.Types.First(t => t.FullName == "Caddx_PCTool.UsbSerialMonitor");
        var method = usbMonitorType.Methods.First(m => m.Name == "ManualSearchDevices");
        var updateSnapshot = usbMonitorType.Methods.First(m => m.Name == "UpdateDeviceSnapshot");

        var usbDevInfoType = module.Types.First(t => t.FullName == "Caddx_PCTool.UsbDevInfo");
        var usbDevInfoCtor = usbDevInfoType.Methods.First(m => m.IsConstructor && !m.IsStatic && m.Parameters.Count == 0);
        var setPortName = usbDevInfoType.Methods.First(m => m.Name == "set_PortName");
        var setVid = usbDevInfoType.Methods.First(m => m.Name == "set_VID");
        var setPid = usbDevInfoType.Methods.First(m => m.Name == "set_PID");
        var setDevName = usbDevInfoType.Methods.First(m => m.Name == "set_DevName");
        var setConnectedTime = usbDevInfoType.Methods.First(m => m.Name == "set_ConnectedTime");

        // Build BCL references manually against the TARGET assembly's own
        // mscorlib/System AssemblyNameReferences. We cannot use typeof()/reflection
        // here because this patcher runs on modern .NET, whose core types live in
        // different assemblies than .NET Framework 4.8's mscorlib/System.
        var mscorlibRef = module.AssemblyReferences.First(a => a.Name == "mscorlib");
        var systemAsmRef = module.AssemblyReferences.First(a => a.Name == "System");

        var listOpenTypeRef = new TypeReference("System.Collections.Generic", "List`1", module, mscorlibRef);
        listOpenTypeRef.GenericParameters.Add(new GenericParameter(listOpenTypeRef));
        var listOfUsbDevInfo = new GenericInstanceType(listOpenTypeRef);
        listOfUsbDevInfo.GenericArguments.Add(usbDevInfoType);

        var listCtor = new MethodReference(".ctor", module.TypeSystem.Void, listOfUsbDevInfo) { HasThis = true };

        var listAdd = new MethodReference("Add", module.TypeSystem.Void, listOfUsbDevInfo) { HasThis = true };
        listAdd.Parameters.Add(new ParameterDefinition(listOpenTypeRef.GenericParameters[0]));

        var boolType = module.TypeSystem.Boolean;
        var intType = module.TypeSystem.Int32;
        var stringType = module.TypeSystem.String;
        var stringArrayType = new ArrayType(stringType);

        var dateTimeTypeRef = new TypeReference("System", "DateTime", module, mscorlibRef) { IsValueType = true };
        var exceptionTypeRef = new TypeReference("System", "Exception", module, mscorlibRef);
        var serialPortTypeRef = new TypeReference("System.IO.Ports", "SerialPort", module, systemAsmRef);

        var getPortNames = new MethodReference("GetPortNames", stringArrayType, serialPortTypeRef) { HasThis = false };

        var stringLenGetter = new MethodReference("get_Length", intType, stringType) { HasThis = true };

        var substring = new MethodReference("Substring", stringType, stringType) { HasThis = true };
        substring.Parameters.Add(new ParameterDefinition(intType));
        substring.Parameters.Add(new ParameterDefinition(intType));

        var substring1 = new MethodReference("Substring", stringType, stringType) { HasThis = true };
        substring1.Parameters.Add(new ParameterDefinition(intType));

        var stringEquals = new MethodReference("op_Equality", boolType, stringType) { HasThis = false };
        stringEquals.Parameters.Add(new ParameterDefinition(stringType));
        stringEquals.Parameters.Add(new ParameterDefinition(stringType));

        var intTryParse = new MethodReference("TryParse", boolType, intType) { HasThis = false };
        intTryParse.Parameters.Add(new ParameterDefinition(stringType));
        intTryParse.Parameters.Add(new ParameterDefinition(new ByReferenceType(intType)) { IsOut = true });

        var dateTimeNowGetter = new MethodReference("get_Now", dateTimeTypeRef, dateTimeTypeRef) { HasThis = false };

        method.Body.Instructions.Clear();
        method.Body.Variables.Clear();
        method.Body.ExceptionHandlers.Clear();

        var vList = new VariableDefinition(listOfUsbDevInfo);
        var vPortNames = new VariableDefinition(stringArrayType);
        var vI = new VariableDefinition(intType);
        var vItem = new VariableDefinition(stringType);
        var vNum = new VariableDefinition(intType);
        var vInfo = new VariableDefinition(usbDevInfoType);
        var vIsLegacy = new VariableDefinition(boolType);
        var vEx = new VariableDefinition(exceptionTypeRef);

        method.Body.Variables.Add(vList);
        method.Body.Variables.Add(vPortNames);
        method.Body.Variables.Add(vI);
        method.Body.Variables.Add(vItem);
        method.Body.Variables.Add(vNum);
        method.Body.Variables.Add(vInfo);
        method.Body.Variables.Add(vIsLegacy);
        method.Body.Variables.Add(vEx);

        var il = method.Body.GetILProcessor();

        var lblTryStart = il.Create(OpCodes.Nop);
        var lblLoopCheck = il.Create(OpCodes.Nop);
        var lblLoopBody = il.Create(OpCodes.Nop);
        var lblSkip = il.Create(OpCodes.Nop);
        var lblAfterFilterCheck = il.Create(OpCodes.Nop);
        var lblLoopIncr = il.Create(OpCodes.Nop);
        var lblAfterLoop = il.Create(OpCodes.Nop);
        var lblCallSnapshot = il.Create(OpCodes.Nop);
        var lblTryEnd = il.Create(OpCodes.Nop);
        var lblCatchStart = il.Create(OpCodes.Nop);
        var lblCatchEnd = il.Create(OpCodes.Nop);
        var lblReturn = il.Create(OpCodes.Ldloc, vList);

        // list = new List<UsbDevInfo>()
        il.Emit(OpCodes.Newobj, listCtor);
        il.Emit(OpCodes.Stloc, vList);

        il.Append(lblTryStart);
        // portNames = SerialPort.GetPortNames()
        il.Emit(OpCodes.Call, getPortNames);
        il.Emit(OpCodes.Stloc, vPortNames);

        // i = 0
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Stloc, vI);
        il.Emit(OpCodes.Br, lblLoopCheck);

        il.Append(lblLoopBody);
        // item = portNames[i]
        il.Emit(OpCodes.Ldloc, vPortNames);
        il.Emit(OpCodes.Ldloc, vI);
        il.Emit(OpCodes.Ldelem_Ref);
        il.Emit(OpCodes.Stloc, vItem);

        // isLegacy = false
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Stloc, vIsLegacy);

        // if (item.Length > 3)
        il.Emit(OpCodes.Ldloc, vItem);
        il.Emit(OpCodes.Callvirt, stringLenGetter);
        il.Emit(OpCodes.Ldc_I4_3);
        il.Emit(OpCodes.Ble, lblAfterFilterCheck);

        // if (item.Substring(0,3) == "COM")
        il.Emit(OpCodes.Ldloc, vItem);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ldc_I4_3);
        il.Emit(OpCodes.Callvirt, substring);
        il.Emit(OpCodes.Ldstr, "COM");
        il.Emit(OpCodes.Call, stringEquals);
        il.Emit(OpCodes.Brfalse, lblAfterFilterCheck);

        // if (int.TryParse(item.Substring(3), out num))
        il.Emit(OpCodes.Ldloc, vItem);
        il.Emit(OpCodes.Ldc_I4_3);
        il.Emit(OpCodes.Callvirt, substring1);
        il.Emit(OpCodes.Ldloca, vNum);
        il.Emit(OpCodes.Call, intTryParse);
        il.Emit(OpCodes.Brfalse, lblAfterFilterCheck);

        // if (num >= 1 && num <= 32) isLegacy = true;
        il.Emit(OpCodes.Ldloc, vNum);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Blt, lblAfterFilterCheck);
        il.Emit(OpCodes.Ldloc, vNum);
        il.Emit(OpCodes.Ldc_I4, 32);
        il.Emit(OpCodes.Bgt, lblAfterFilterCheck);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Stloc, vIsLegacy);

        il.Append(lblAfterFilterCheck);
        il.Emit(OpCodes.Ldloc, vIsLegacy);
        il.Emit(OpCodes.Brtrue, lblSkip);

        // info = new UsbDevInfo(); info.PortName = item; info.VID="1D76"; info.PID="0101"; info.DevName="..."; info.ConnectedTime = DateTime.Now;
        il.Emit(OpCodes.Newobj, usbDevInfoCtor);
        il.Emit(OpCodes.Stloc, vInfo);

        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Ldloc, vItem);
        il.Emit(OpCodes.Callvirt, setPortName);

        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Ldstr, "1D76");
        il.Emit(OpCodes.Callvirt, setVid);

        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Ldstr, "0101");
        il.Emit(OpCodes.Callvirt, setPid);

        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Ldstr, "Caddx Ascent (Linux/Wine bridge patch)");
        il.Emit(OpCodes.Callvirt, setDevName);

        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Call, dateTimeNowGetter);
        il.Emit(OpCodes.Callvirt, setConnectedTime);

        // list.Add(info)
        il.Emit(OpCodes.Ldloc, vList);
        il.Emit(OpCodes.Ldloc, vInfo);
        il.Emit(OpCodes.Callvirt, listAdd);

        il.Append(lblSkip);
        il.Append(lblLoopIncr);
        il.Emit(OpCodes.Ldloc, vI);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Stloc, vI);

        il.Append(lblLoopCheck);
        il.Emit(OpCodes.Ldloc, vI);
        il.Emit(OpCodes.Ldloc, vPortNames);
        il.Emit(OpCodes.Ldlen);
        il.Emit(OpCodes.Conv_I4);
        il.Emit(OpCodes.Blt, lblLoopBody);

        il.Append(lblAfterLoop);

        // --- Patch 5 (wire-capture-harness only, opt-in via CADDX_FAKE_PORT) ---
        // Wine's SerialPort.GetPortNames() only ever returns COM1-32 (the static
        // legacy ttyS0-31 mapping) plus whatever COMn it dynamically assigns for
        // real, udev-detected USB-serial hardware — confirmed empirically (a
        // minimal GetPortNames() probe compiled with the prefix's own csc.exe)
        // that this list is regenerated live and excludes anything else,
        // regardless of dosdevices/comNN symlinks or manual
        // HKLM\HARDWARE\DEVICEMAP\SERIALCOMM registry edits pointing at one (that
        // key is itself volatile/synthesized fresh, not read back from what was
        // written). So a socat-created PTY — used to run this exe against
        // CaddxTool.FakeDevice for wire-protocol comparison, see
        // src/PROJECT.md's "Wire-capture harness" — never shows up on its own.
        //
        // Merely appending a synthetic UsbDevInfo to the returned `list` (as an
        // earlier version of this patch did) is not enough on its own: that list
        // feeds UpdateDeviceSnapshot(list), which calls AddDevice() for anything
        // not already tracked, which schedules VerifyDeviceAvailability() 300ms
        // later — and THAT method calls the raw, unpatched SerialPort.
        // GetPortNames() again, fails to find our fake port there (same root
        // cause as above), and calls RemoveDevice(), firing a "device unplugged"
        // event and killing the session before a single byte is ever sent.
        // Confirmed via the app's own Log/*.log ("拔出设备,串口名=COM50" right
        // after "启动wmi监听") and an empty CaddxTool.FakeDevice capture log.
        // Patch 6 (below) guards RemoveDevice as defense-in-depth against the
        // separate broken WMI OnDeviceRemoved watcher, but doesn't fix this: a
        // no-op RemoveDevice still leaves VerifyDeviceAvailability's caller
        // returning early right after calling it, before ever reaching
        // DeviceConnected?.Invoke(...) — so the auto-handshake still never
        // starts. So instead of relying on the normal AddDevice/
        // VerifyDeviceAvailability path at all, this block does that path's
        // *entire* job itself, synchronously, right here: registers the fake
        // device directly in _connectedDevices/_portToDeviceIdMap and fires
        // DeviceConnected immediately. That has a useful side effect beyond just
        // skipping the bad check: since _connectedDevices already contains our
        // port by the time UpdateDeviceSnapshot(list) runs (just below), its own
        // `if (!_connectedDevices.ContainsKey(...)) AddDevice(...)` check is
        // false for our entry, so AddDevice/VerifyDeviceAvailability never gets
        // scheduled for it at all. No effect on normal real-hardware usage
        // unless CADDX_FAKE_PORT is set.
        {
            var environmentTypeRef5 = new TypeReference("System", "Environment", module, mscorlibRef);
            var getEnvVar5 = new MethodReference("GetEnvironmentVariable", stringType, environmentTypeRef5) { HasThis = false };
            getEnvVar5.Parameters.Add(new ParameterDefinition(stringType));
            var stringIsNullOrEmpty = new MethodReference("IsNullOrEmpty", boolType, stringType) { HasThis = false };
            stringIsNullOrEmpty.Parameters.Add(new ParameterDefinition(stringType));

            var vFakePort = new VariableDefinition(stringType);
            method.Body.Variables.Add(vFakePort);

            var lblSkipFakePort = il.Create(OpCodes.Nop);

            il.Emit(OpCodes.Ldstr, "CADDX_FAKE_PORT");
            il.Emit(OpCodes.Call, getEnvVar5);
            il.Emit(OpCodes.Stloc, vFakePort);

            il.Emit(OpCodes.Ldloc, vFakePort);
            il.Emit(OpCodes.Call, stringIsNullOrEmpty);
            il.Emit(OpCodes.Brtrue, lblSkipFakePort);

            il.Emit(OpCodes.Newobj, usbDevInfoCtor);
            il.Emit(OpCodes.Stloc, vInfo);

            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Ldloc, vFakePort);
            il.Emit(OpCodes.Callvirt, setPortName);

            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Ldstr, "1D76");
            il.Emit(OpCodes.Callvirt, setVid);

            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Ldstr, "0101");
            il.Emit(OpCodes.Callvirt, setPid);

            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Ldstr, "Caddx Ascent (wire-capture test port)");
            il.Emit(OpCodes.Callvirt, setDevName);

            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Call, dateTimeNowGetter);
            il.Emit(OpCodes.Callvirt, setConnectedTime);

            il.Emit(OpCodes.Ldloc, vList);
            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Callvirt, listAdd);

            // --- Directly do AddDevice()+VerifyDeviceAvailability()'s success-path
            // job ourselves: _connectedDevices.TryAdd(...), _portToDeviceIdMap[...] =
            // ..., info.IsInserted = true, DeviceConnected?.Invoke(this, info). ---
            var concurrentDictOpenRef = new TypeReference("System.Collections.Concurrent", "ConcurrentDictionary`2", module, mscorlibRef);
            concurrentDictOpenRef.GenericParameters.Add(new GenericParameter("TKey", concurrentDictOpenRef));
            concurrentDictOpenRef.GenericParameters.Add(new GenericParameter("TValue", concurrentDictOpenRef));

            var connectedDevicesDictType = new GenericInstanceType(concurrentDictOpenRef);
            connectedDevicesDictType.GenericArguments.Add(stringType);
            connectedDevicesDictType.GenericArguments.Add(usbDevInfoType);
            var tryAddConnected = new MethodReference("TryAdd", boolType, connectedDevicesDictType) { HasThis = true };
            tryAddConnected.Parameters.Add(new ParameterDefinition(concurrentDictOpenRef.GenericParameters[0]));
            tryAddConnected.Parameters.Add(new ParameterDefinition(concurrentDictOpenRef.GenericParameters[1]));

            var portMapDictType = new GenericInstanceType(concurrentDictOpenRef);
            portMapDictType.GenericArguments.Add(stringType);
            portMapDictType.GenericArguments.Add(stringType);
            var setItemPortMap = new MethodReference("set_Item", module.TypeSystem.Void, portMapDictType) { HasThis = true };
            setItemPortMap.Parameters.Add(new ParameterDefinition(concurrentDictOpenRef.GenericParameters[0]));
            setItemPortMap.Parameters.Add(new ParameterDefinition(concurrentDictOpenRef.GenericParameters[1]));

            var setIsInserted = usbDevInfoType.Methods.First(m => m.Name == "set_IsInserted");

            var eventHandlerOpenRef = new TypeReference("System", "EventHandler`1", module, mscorlibRef);
            eventHandlerOpenRef.GenericParameters.Add(new GenericParameter("T", eventHandlerOpenRef));
            var deviceConnectedHandlerType = new GenericInstanceType(eventHandlerOpenRef);
            deviceConnectedHandlerType.GenericArguments.Add(usbDevInfoType);
            var invokeDeviceConnected = new MethodReference("Invoke", module.TypeSystem.Void, deviceConnectedHandlerType) { HasThis = true };
            invokeDeviceConnected.Parameters.Add(new ParameterDefinition(module.TypeSystem.Object));
            invokeDeviceConnected.Parameters.Add(new ParameterDefinition(eventHandlerOpenRef.GenericParameters[0]));

            var connectedDevicesField = usbMonitorType.Fields.First(f => f.Name == "_connectedDevices");
            var portToDeviceIdMapField = usbMonitorType.Fields.First(f => f.Name == "_portToDeviceIdMap");
            var deviceConnectedField = usbMonitorType.Fields.First(f => f.Name == "DeviceConnected");

            // _connectedDevices.TryAdd(fakePort, info); (discard bool result)
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, connectedDevicesField);
            il.Emit(OpCodes.Ldloc, vFakePort);
            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Callvirt, tryAddConnected);
            il.Emit(OpCodes.Pop);

            // _portToDeviceIdMap[fakePort] = fakePort;
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, portToDeviceIdMapField);
            il.Emit(OpCodes.Ldloc, vFakePort);
            il.Emit(OpCodes.Ldloc, vFakePort);
            il.Emit(OpCodes.Callvirt, setItemPortMap);

            // info.IsInserted = true;
            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Callvirt, setIsInserted);

            // DeviceConnected?.Invoke(this, info);
            var lblDeviceConnectedNull = il.Create(OpCodes.Nop);
            var lblAfterDeviceConnected = il.Create(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, deviceConnectedField);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brtrue, lblDeviceConnectedNull);
            il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Br, lblAfterDeviceConnected);
            il.Append(lblDeviceConnectedNull);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, vInfo);
            il.Emit(OpCodes.Callvirt, invokeDeviceConnected);
            il.Append(lblAfterDeviceConnected);

            il.Append(lblSkipFakePort);
        }

        // if (_isMonitoring) UpdateDeviceSnapshot(list);
        il.Emit(OpCodes.Ldarg_0);
        var isMonitoringField = usbMonitorType.Fields.First(f => f.Name == "_isMonitoring");
        il.Emit(OpCodes.Ldfld, isMonitoringField);
        il.Emit(OpCodes.Brfalse, lblTryEnd);

        il.Append(lblCallSnapshot);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldloc, vList);
        il.Emit(OpCodes.Call, updateSnapshot);

        il.Append(lblTryEnd);
        il.Emit(OpCodes.Leave, lblReturn);

        il.Append(lblCatchStart);
        il.Emit(OpCodes.Stloc, vEx);
        il.Emit(OpCodes.Nop);
        il.Append(lblCatchEnd);
        il.Emit(OpCodes.Leave, lblReturn);

        il.Append(lblReturn);
        il.Emit(OpCodes.Ret);

        var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
        {
            CatchType = exceptionTypeRef,
            TryStart = lblTryStart,
            TryEnd = lblCatchStart,
            HandlerStart = lblCatchStart,
            HandlerEnd = lblReturn
        };
        method.Body.ExceptionHandlers.Add(handler);

        method.Body.InitLocals = true;

        // --- Patch 6 (wire-capture-harness only, opt-in via CADDX_FAKE_PORT) ---
        // Patch 5 gets the synthetic port into the initial ManualSearchDevices()
        // scan, but that's not the end of the story: UsbSerialMonitor.AddDevice()
        // schedules VerifyDeviceAvailability(deviceInfo) 300ms later, which calls
        // the RAW, still-unpatched SerialPort.GetPortNames() again to double-check
        // the port is still there — and since that raw call obviously never
        // includes our fake port (see the empirical GetPortNames() finding in
        // Patch 5's comment above), it immediately calls RemoveDevice(), firing a
        // "device unplugged" event and killing the session before a single byte
        // gets sent. Confirmed via the app's own Log/*.log ("拔出设备,串口名=COM50"
        // right after "启动wmi监听") and an empty CaddxTool.FakeDevice capture log
        // — zero traffic ever reached the fake device.
        // RemoveDevice(string deviceId, string portName) is the single common
        // choke point every removal path goes through (VerifyDeviceAvailability,
        // UpdateDeviceSnapshot's diffing, and the separate broken WMI
        // OnDeviceRemoved watcher all call it) — much simpler to guard here than
        // to patch VerifyDeviceAvailability's compiler-generated async state
        // machine directly. No effect on normal real-hardware usage: the guard
        // only no-ops when portName matches CADDX_FAKE_PORT.
        {
            var removeDeviceMethod = usbMonitorType.Methods.First(m => m.Name == "RemoveDevice");
            var rdIl = removeDeviceMethod.Body.GetILProcessor();
            var firstInstr = removeDeviceMethod.Body.Instructions.First();

            var environmentTypeRef6 = new TypeReference("System", "Environment", module, mscorlibRef);
            var getEnvVar6 = new MethodReference("GetEnvironmentVariable", stringType, environmentTypeRef6) { HasThis = false };
            getEnvVar6.Parameters.Add(new ParameterDefinition(stringType));
            var stringIsNullOrEmpty6 = new MethodReference("IsNullOrEmpty", boolType, stringType) { HasThis = false };
            stringIsNullOrEmpty6.Parameters.Add(new ParameterDefinition(stringType));

            var vFakePort6 = new VariableDefinition(stringType);
            removeDeviceMethod.Body.Variables.Add(vFakePort6);

            var lblSkipGuard = firstInstr; // reuse as branch target (insert before it)

            var newInstrs = new[]
            {
                // fakePort = Environment.GetEnvironmentVariable("CADDX_FAKE_PORT")
                rdIl.Create(OpCodes.Ldstr, "CADDX_FAKE_PORT"),
                rdIl.Create(OpCodes.Call, getEnvVar6),
                rdIl.Create(OpCodes.Stloc, vFakePort6),
                // if (string.IsNullOrEmpty(fakePort)) goto normal method body;
                rdIl.Create(OpCodes.Ldloc, vFakePort6),
                rdIl.Create(OpCodes.Call, stringIsNullOrEmpty6),
                rdIl.Create(OpCodes.Brtrue, lblSkipGuard),
                // if (portName != fakePort) goto normal method body;
                rdIl.Create(OpCodes.Ldarg_2),
                rdIl.Create(OpCodes.Ldloc, vFakePort6),
                rdIl.Create(OpCodes.Call, stringEquals),
                rdIl.Create(OpCodes.Brfalse, lblSkipGuard),
                // it's our fake device — do nothing, don't let it be removed
                rdIl.Create(OpCodes.Ret),
            };
            foreach (var instr in newInstrs)
            {
                rdIl.InsertBefore(firstInstr, instr);
            }
            Console.WriteLine("Inserted CADDX_FAKE_PORT removal guard into UsbSerialMonitor.RemoveDevice().");
        }

        // --- Same WMI-bypass fix, applied to updateChannel.ManualSearchDevices ---
        // The bb_freq screen (updateChannel.cs) has its own separate, duplicated
        // ManualSearchDevices(out List<UsbDevInfo>) implementation with the exact
        // same broken WMI Win32_PnPEntity lookup — it's not shared with
        // UsbSerialMonitor, so the earlier patch doesn't cover it. Same technique,
        // reusing the type/method references already built above, adapted to this
        // method's `bool ManualSearchDevices(out List<UsbDevInfo> usbinfo)` signature.
        {
            var updateChannelType = module.Types.First(t => t.FullName == "Caddx_PCTool.updateChannel");
            var ucMethod = updateChannelType.Methods.First(m => m.Name == "ManualSearchDevices" && m.Parameters.Count == 1);

            var listCount = new MethodReference("get_Count", intType, listOfUsbDevInfo) { HasThis = true };

            ucMethod.Body.Instructions.Clear();
            ucMethod.Body.Variables.Clear();
            ucMethod.Body.ExceptionHandlers.Clear();

            var uList = new VariableDefinition(listOfUsbDevInfo);
            var uPortNames = new VariableDefinition(stringArrayType);
            var uI = new VariableDefinition(intType);
            var uItem = new VariableDefinition(stringType);
            var uNum = new VariableDefinition(intType);
            var uInfo = new VariableDefinition(usbDevInfoType);
            var uIsLegacy = new VariableDefinition(boolType);
            var uEx = new VariableDefinition(exceptionTypeRef);
            var uResult = new VariableDefinition(boolType);

            ucMethod.Body.Variables.Add(uList);
            ucMethod.Body.Variables.Add(uPortNames);
            ucMethod.Body.Variables.Add(uI);
            ucMethod.Body.Variables.Add(uItem);
            ucMethod.Body.Variables.Add(uNum);
            ucMethod.Body.Variables.Add(uInfo);
            ucMethod.Body.Variables.Add(uIsLegacy);
            ucMethod.Body.Variables.Add(uEx);
            ucMethod.Body.Variables.Add(uResult);

            var uil = ucMethod.Body.GetILProcessor();

            var uTryStart = uil.Create(OpCodes.Nop);
            var uLoopCheck = uil.Create(OpCodes.Nop);
            var uLoopBody = uil.Create(OpCodes.Nop);
            var uSkip = uil.Create(OpCodes.Nop);
            var uAfterFilterCheck = uil.Create(OpCodes.Nop);
            var uLoopIncr = uil.Create(OpCodes.Nop);
            var uAfterLoop = uil.Create(OpCodes.Nop);
            var uTryEnd = uil.Create(OpCodes.Nop);
            var uCatchStart = uil.Create(OpCodes.Nop);
            var uWriteOut = uil.Create(OpCodes.Ldarg_1);
            // Shared exit point OUTSIDE the try/catch region. CIL forbids a bare
            // `ret` inside a protected (try/catch) region — every exit must go
            // through `leave` to a point outside it. Both the normal-completion
            // path and the catch handler set uResult and `leave` here.
            var uReturn = uil.Create(OpCodes.Ldloc, uResult);

            uil.Emit(OpCodes.Newobj, listCtor);
            uil.Emit(OpCodes.Stloc, uList);

            uil.Append(uTryStart);
            uil.Emit(OpCodes.Call, getPortNames);
            uil.Emit(OpCodes.Stloc, uPortNames);

            uil.Emit(OpCodes.Ldc_I4_0);
            uil.Emit(OpCodes.Stloc, uI);
            uil.Emit(OpCodes.Br, uLoopCheck);

            uil.Append(uLoopBody);
            uil.Emit(OpCodes.Ldloc, uPortNames);
            uil.Emit(OpCodes.Ldloc, uI);
            uil.Emit(OpCodes.Ldelem_Ref);
            uil.Emit(OpCodes.Stloc, uItem);

            uil.Emit(OpCodes.Ldc_I4_0);
            uil.Emit(OpCodes.Stloc, uIsLegacy);

            uil.Emit(OpCodes.Ldloc, uItem);
            uil.Emit(OpCodes.Callvirt, stringLenGetter);
            uil.Emit(OpCodes.Ldc_I4_3);
            uil.Emit(OpCodes.Ble, uAfterFilterCheck);

            uil.Emit(OpCodes.Ldloc, uItem);
            uil.Emit(OpCodes.Ldc_I4_0);
            uil.Emit(OpCodes.Ldc_I4_3);
            uil.Emit(OpCodes.Callvirt, substring);
            uil.Emit(OpCodes.Ldstr, "COM");
            uil.Emit(OpCodes.Call, stringEquals);
            uil.Emit(OpCodes.Brfalse, uAfterFilterCheck);

            uil.Emit(OpCodes.Ldloc, uItem);
            uil.Emit(OpCodes.Ldc_I4_3);
            uil.Emit(OpCodes.Callvirt, substring1);
            uil.Emit(OpCodes.Ldloca, uNum);
            uil.Emit(OpCodes.Call, intTryParse);
            uil.Emit(OpCodes.Brfalse, uAfterFilterCheck);

            uil.Emit(OpCodes.Ldloc, uNum);
            uil.Emit(OpCodes.Ldc_I4_1);
            uil.Emit(OpCodes.Blt, uAfterFilterCheck);
            uil.Emit(OpCodes.Ldloc, uNum);
            uil.Emit(OpCodes.Ldc_I4, 32);
            uil.Emit(OpCodes.Bgt, uAfterFilterCheck);
            uil.Emit(OpCodes.Ldc_I4_1);
            uil.Emit(OpCodes.Stloc, uIsLegacy);

            uil.Append(uAfterFilterCheck);
            uil.Emit(OpCodes.Ldloc, uIsLegacy);
            uil.Emit(OpCodes.Brtrue, uSkip);

            uil.Emit(OpCodes.Newobj, usbDevInfoCtor);
            uil.Emit(OpCodes.Stloc, uInfo);

            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Ldloc, uItem);
            uil.Emit(OpCodes.Callvirt, setPortName);

            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Ldstr, "1D76");
            uil.Emit(OpCodes.Callvirt, setVid);

            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Ldstr, "0101");
            uil.Emit(OpCodes.Callvirt, setPid);

            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Ldstr, "Caddx Ascent (Linux/Wine bridge patch)");
            uil.Emit(OpCodes.Callvirt, setDevName);

            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Call, dateTimeNowGetter);
            uil.Emit(OpCodes.Callvirt, setConnectedTime);

            uil.Emit(OpCodes.Ldloc, uList);
            uil.Emit(OpCodes.Ldloc, uInfo);
            uil.Emit(OpCodes.Callvirt, listAdd);

            uil.Append(uSkip);
            uil.Append(uLoopIncr);
            uil.Emit(OpCodes.Ldloc, uI);
            uil.Emit(OpCodes.Ldc_I4_1);
            uil.Emit(OpCodes.Add);
            uil.Emit(OpCodes.Stloc, uI);

            uil.Append(uLoopCheck);
            uil.Emit(OpCodes.Ldloc, uI);
            uil.Emit(OpCodes.Ldloc, uPortNames);
            uil.Emit(OpCodes.Ldlen);
            uil.Emit(OpCodes.Conv_I4);
            uil.Emit(OpCodes.Blt, uLoopBody);

            uil.Append(uAfterLoop);
            uil.Append(uTryEnd);
            uil.Emit(OpCodes.Leave, uWriteOut);

            uil.Append(uCatchStart);
            uil.Emit(OpCodes.Stloc, uEx);
            // catch block: usbinfo = new List<UsbDevInfo>(); return false;
            uil.Emit(OpCodes.Ldarg_1);
            uil.Emit(OpCodes.Newobj, listCtor);
            uil.Emit(OpCodes.Stind_Ref);
            uil.Emit(OpCodes.Ldc_I4_0);
            uil.Emit(OpCodes.Stloc, uResult);
            uil.Emit(OpCodes.Leave, uReturn);

            // *usbinfo = uList; uResult = uList.Count > 0;
            uil.Append(uWriteOut); // ldarg.1
            uil.Emit(OpCodes.Ldloc, uList);
            uil.Emit(OpCodes.Stind_Ref);
            uil.Emit(OpCodes.Ldloc, uList);
            uil.Emit(OpCodes.Callvirt, listCount);
            uil.Emit(OpCodes.Ldc_I4_0);
            uil.Emit(OpCodes.Cgt);
            uil.Emit(OpCodes.Stloc, uResult);
            uil.Append(uReturn); // ldloc uResult
            uil.Emit(OpCodes.Ret);

            var uHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = exceptionTypeRef,
                TryStart = uTryStart,
                TryEnd = uCatchStart,
                HandlerStart = uCatchStart,
                HandlerEnd = uWriteOut
            };
            ucMethod.Body.ExceptionHandlers.Add(uHandler);
            ucMethod.Body.InitLocals = true;

            Console.WriteLine("Patched updateChannel.ManualSearchDevices().");
        }

        // --- Optional power-user unlock, gated by env var CADDX_POWER_USER=1 ---
        // Prepends a few instructions to Program.Main():
        //   if (Environment.GetEnvironmentVariable("CADDX_POWER_USER") == "1")
        //       GD.Inst.CurrUserLevel = UserLevel.caddx;
        // Same binary works for both normal and power-user runs; nothing changes
        // unless the env var is set, so this is purely additive/opt-in.
        {
            var programType = module.Types.First(t => t.FullName == "Caddx_PCTool.Program");
            var mainMethod = programType.Methods.First(m => m.Name == "Main");
            var gdType = module.Types.First(t => t.FullName == "Caddx_PCTool.GD");
            var getInst = gdType.Methods.First(m => m.Name == "get_Inst");
            var setCurrUserLevel = gdType.Methods.First(m => m.Name == "set_CurrUserLevel");
            var writeLogType = module.Types.First(t => t.FullName == "Caddx_PCTool.WriteLog");
            var writeLogMethod = writeLogType.Methods.First(m => m.Name == "writeLog" && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "System.String");

            var environmentTypeRef = new TypeReference("System", "Environment", module, mscorlibRef);
            var getEnvVar = new MethodReference("GetEnvironmentVariable", stringType, environmentTypeRef) { HasThis = false };
            getEnvVar.Parameters.Add(new ParameterDefinition(stringType));

            var stringConcat3 = new MethodReference("Concat", stringType, stringType) { HasThis = false };
            stringConcat3.Parameters.Add(new ParameterDefinition(stringType));
            stringConcat3.Parameters.Add(new ParameterDefinition(stringType));
            stringConcat3.Parameters.Add(new ParameterDefinition(stringType));

            var mainIl = mainMethod.Body.GetILProcessor();
            // Anchor right before "new MainFrm()" — inserting at the very start of
            // Main() doesn't work: Program.ReadParam() (called from LoadSplashScreen(),
            // which runs earlier in Main()) deserializes a saved GD object from disk
            // and replaces the whole GD.Inst singleton (_inst = myGD). CurrUserLevel is
            // [JsonIgnore]'d, so that replacement silently resets it back to the default
            // (UserLevel.op), wiping out an earlier unlock. Setting it after that
            // replacement, right before MainFrm is constructed, survives.
            var firstInstr = mainMethod.Body.Instructions.First(i =>
                i.OpCode == OpCodes.Newobj &&
                i.Operand is MethodReference mr &&
                mr.DeclaringType.Name == "MainFrm");
            var skipUnlock = firstInstr; // reuse as branch target (insert before it)

            var vEnvVal = new VariableDefinition(stringType);
            mainMethod.Body.Variables.Add(vEnvVal);

            var newInstrs = new[]
            {
                // vEnvVal = Environment.GetEnvironmentVariable("CADDX_POWER_USER")
                mainIl.Create(OpCodes.Ldstr, "CADDX_POWER_USER"),
                mainIl.Create(OpCodes.Call, getEnvVar),
                mainIl.Create(OpCodes.Stloc, vEnvVal),
                // WriteLog.writeLog("POWERUSER_ENV=[" + vEnvVal + "]")
                mainIl.Create(OpCodes.Ldstr, "POWERUSER_ENV=["),
                mainIl.Create(OpCodes.Ldloc, vEnvVal),
                mainIl.Create(OpCodes.Ldstr, "]"),
                mainIl.Create(OpCodes.Call, stringConcat3),
                mainIl.Create(OpCodes.Call, writeLogMethod),
                // if (vEnvVal == "1") { GD.Inst.CurrUserLevel = UserLevel.caddx; WriteLog.writeLog("POWERUSER_UNLOCK_APPLIED"); }
                mainIl.Create(OpCodes.Ldloc, vEnvVal),
                mainIl.Create(OpCodes.Ldstr, "1"),
                mainIl.Create(OpCodes.Call, stringEquals),
                mainIl.Create(OpCodes.Brfalse, skipUnlock),
                mainIl.Create(OpCodes.Call, getInst),
                mainIl.Create(OpCodes.Ldc_I4_1), // UserLevel.caddx == 1
                mainIl.Create(OpCodes.Callvirt, setCurrUserLevel),
                mainIl.Create(OpCodes.Ldstr, "POWERUSER_UNLOCK_APPLIED"),
                mainIl.Create(OpCodes.Call, writeLogMethod),
            };
            foreach (var instr in newInstrs)
            {
                mainIl.InsertBefore(firstInstr, instr);
            }
            Console.WriteLine("Inserted power-user unlock (with diagnostic logging) into Program.Main().");
        }

        // --- Patch 4: drop the "consumer build" device-allowlist gate ---
        // AscentDeviceNameResolver.ResolveDisplayName() only trusts the full
        // device-name lookup table when IsConsumerVersion(Program.SoftwareVersion)
        // is false. That check looks for a "_C" marker in the version string —
        // this retail installer ships v2.2.9_C, so it's always true here. For
        // "consumer" builds, ResolveDisplayName instead requires the device name
        // to appear in a small hardcoded allowlist (ConsumerAllowedDeviceNameKeys)
        // that Caddx never updated for newer models — "ascent_gt_pro" (and others,
        // e.g. its z40/z8/hub variants) aren't in it, even though the rest of the
        // resolution/display code for those models (name mapping, icon resources)
        // is fully present and functional. Rather than allowlisting GT Pro
        // specifically, make IsConsumerVersion() unconditionally return false, so
        // ResolveDisplayName always takes the same path non-consumer builds do
        // (ResolveFromDeviceName first, falling back to ResolveFromVersionFields)
        // — this unlocks every device the resolver already knows how to name.
        {
            var resolverType = module.Types.First(t => t.FullName == "Caddx_PCTool.AscentDeviceNameResolver");
            var isConsumerVersionMethod = resolverType.Methods.First(m => m.Name == "IsConsumerVersion");

            isConsumerVersionMethod.Body.Instructions.Clear();
            isConsumerVersionMethod.Body.Variables.Clear();
            isConsumerVersionMethod.Body.ExceptionHandlers.Clear();

            var rIl = isConsumerVersionMethod.Body.GetILProcessor();
            rIl.Emit(OpCodes.Ldc_I4_0);
            rIl.Emit(OpCodes.Ret);

            Console.WriteLine("Patched AscentDeviceNameResolver.IsConsumerVersion() to always return false.");
        }

        // --- Patch 7: widen the firmware-upgrade ack-retry/timeout constants ---
        // UpgradeProcessFSM.SendWithRetryGuard/SendWithRetryGuardDelay (the two
        // wrappers every upgrade-flow command funnels through: RemoteUpgrade,
        // SendFileStart/Data/End, UpgradeStatus, GetBbFreq, SetBbFreq,
        // FactoryReset) hardcode retryIntervalMs=2000, totalTimeoutMs=8000,
        // sendTimeoutMs=3000 as bare int literals at their one call site each to
        // UsbSerialportFSM.SendWithAckGuard(Delay) — confirmed via decompiled
        // source, not named consts (the class-level AckRetryIntervalMs/
        // AckTotalTimeoutMs consts exist but are dead, inlined nowhere).
        //
        // Those numbers assume a real serial link, where an ack for even a 1MB
        // SENDFILE_DATA chunk (the real device's own advertised ReceiveMaxSize)
        // comes back in well under a second. That holds for real USB-CDC-ACM
        // hardware, which transfers at USB speed regardless of the nominal
        // "baud rate" — but running this app under Wine against a
        // wire-capture-harness fake device (see src/PROJECT.md's "Wire-capture
        // harness") hits a different wall entirely: Wine's own serial-write path
        // paces SerialPort.Write() to the *configured* baud rate even when the
        // backing device (a tty0tty null-modem pair, needed since plain socat
        // PTYs fail ioctl(TIOCMGET) and never even open) could move the bytes
        // instantly. Confirmed empirically with a minimal compiled probe:
        // writing 200,000 bytes at 115200 baud took ~17.4s under Wine — exact
        // UART timing, not a device-side delay. Even at the highest baud Wine/
        // .NET's SerialPort will accept here (131072 — anything above throws
        // ArgumentOutOfRangeException), a full 1MB chunk still takes ~80s to
        // write, blowing straight through the original 8000ms total-ack-wait
        // regardless of retries.
        //
        // Widening these three constants is the only lever left on the app side
        // (the transport's actual byte-for-byte speed can't be fixed without
        // fixing Wine's serial emulation itself). Applied unconditionally, not
        // gated behind CADDX_FAKE_PORT like Patches 5/6: unlike those two, this
        // one only ever *widens* a wait — real hardware still acks near-
        // instantly and never notices the difference, so there's no behavior
        // change to guard against on a genuine device.
        //
        // First attempt used retryIntervalMs=5000/totalTimeoutMs=180000/
        // sendTimeoutMs=120000 and still failed — root cause: at 115200 baud a
        // full 1MB chunk write alone takes ~91s (SendWithAckGuard's blocking
        // Write() call has to finish before it even starts waiting for an ack),
        // so a 5000ms retryIntervalMs gives up and resends the *entire* chunk
        // long before the first attempt's ack could physically have arrived —
        // confirmed in the capture log (seq=4 retry=1 resending the same full
        // 1,048,576-byte payload). retryIntervalMs must itself exceed the
        // worst-case per-chunk write time, not just totalTimeoutMs. Revised
        // with real margin over the ~91s floor: retryIntervalMs 2000->200000,
        // totalTimeoutMs 8000->600000 (room for ~3 real attempts if ever
        // needed), sendTimeoutMs 3000->200000 (must also clear the ~91s
        // blocking-write floor, since it gates that same Write() call).
        {
            var upgradeFsmType = module.Types.First(t => t.FullName == "Caddx_PCTool.UpgradeProcessFSM");
            var retryGuardMethods = new[]
            {
                upgradeFsmType.Methods.First(m => m.Name == "SendWithRetryGuard"),
                upgradeFsmType.Methods.First(m => m.Name == "SendWithRetryGuardDelay"),
            };

            int patchedTriplets = 0;
            foreach (var m in retryGuardMethods)
            {
                patchedTriplets += RewriteLiteral(m, 2000, 200000);
                patchedTriplets += RewriteLiteral(m, 8000, 600000);
                patchedTriplets += RewriteLiteral(m, 3000, 200000);
            }

            if (patchedTriplets != 6) // 3 literals x 2 methods
            {
                throw new InvalidOperationException(
                    "Patch 7: expected exactly 6 literal rewrites (3 per method x 2 methods), got " + patchedTriplets +
                    " — UpgradeProcessFSM's IL shape no longer matches what this patch assumes; re-check the decompiled source.");
            }

            Console.WriteLine("Widened SendWithRetryGuard/SendWithRetryGuardDelay ack timeouts (2000/8000/3000ms -> 200000/600000/200000ms).");
        }

        // Strip default-value constants whose type lives in assemblies we can't
        // reliably resolve here (e.g. System.IO.Ports.Handshake). This is safe:
        // already-compiled call sites always pass arguments explicitly, so the
        // "default value" metadata is unused at runtime; it only matters for
        // callers relying on the C# compiler's optional-parameter sugar, which
        // doesn't apply to a binary we're patching post-compile.
        int strippedCount = 0;
        foreach (var t in module.Types)
        {
            foreach (var m in t.Methods)
            {
                if (!m.HasBody && !m.HasParameters) continue;
                foreach (var p in m.Parameters)
                {
                    if (p.HasConstant)
                    {
                        var ns = p.ParameterType?.Namespace ?? "";
                        if (ns.StartsWith("System.IO.Ports"))
                        {
                            p.Attributes &= ~Mono.Cecil.ParameterAttributes.HasDefault;
                            p.Attributes &= ~Mono.Cecil.ParameterAttributes.Optional;
                            p.Constant = null;
                            strippedCount++;
                        }
                    }
                }
            }
        }
        Console.WriteLine("Stripped " + strippedCount + " unresolvable default-value constants.");

        var writeParams = new WriterParameters { };
        asm.Write(outPath, writeParams);
        Console.WriteLine("Patched OK -> " + outPath);
        return 0;
    }

    // Rewrites every `ldc.i4 oldValue` instruction operand in method's body to
    // newValue. Ldc_I4 (not Ldc_I4_S) is expected here since all values patched
    // via this helper are outside the sbyte short-form range. Returns the number
    // of instructions rewritten, so callers can assert the expected count.
    static int RewriteLiteral(MethodDefinition method, int oldValue, int newValue)
    {
        int count = 0;
        foreach (var instr in method.Body.Instructions)
        {
            if (instr.OpCode == OpCodes.Ldc_I4 && instr.Operand is int i && i == oldValue)
            {
                instr.Operand = newValue;
                count++;
            }
        }
        return count;
    }
}
