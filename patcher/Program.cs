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
}
