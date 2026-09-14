namespace CaddxTool.Protocol.Tests;

public class FirmwareNameValidatorTests
{
    // Real FirmwareInfo strings observed live (see FakeDeviceProfile.cs).
    [Theory]
    [InlineData("Ascent_H_Sky_18_21_10", "Ascent_H_Sky_18_21_10.img", false)]
    [InlineData("Ascent_VRX_Pro_18_21_7", "Ascent_VRX_Pro_18_21_7.img", false)]
    [InlineData("Ascent_G_Sky_17_5_15", "Ascent_G_Sky_17_5_15.img", false)]
    // Wrong board entirely.
    [InlineData("Ascent_H_Sky_18_21_10", "Ascent_VRX_Pro_18_21_7.img", true)]
    [InlineData("Ascent_VRX_Pro_18_21_7", "Ascent_G_Sky_17_5_15.img", true)]
    [InlineData("Ascent_G_Sky_17_5_15", "Ascent_H_Sky_18_21_10.img", true)]
    // Unrelated filename.
    [InlineData("Ascent_H_Sky_18_21_10", "firmware.img", true)]
    public void LooksMismatched_MatchesVendorPrefixLogic(string firmwareInfo, string fileName, bool expectedMismatch)
    {
        Assert.Equal(expectedMismatch, FirmwareNameValidator.LooksMismatched(fileName, firmwareInfo));
    }

    [Theory]
    [InlineData("Ascent_H_Sky_18_21_10", "Ascent_H_Sky")]
    [InlineData("Ascent_G_Sky_17_5_15", "Ascent_G_Sky")]
    // Truncates mid-word for this one — matches the vendor's own quirk.
    [InlineData("Ascent_VRX_Pro_18_21_7", "Ascent_VRX_P")]
    public void ExpectedPrefix_MatchesVendorLogic(string firmwareInfo, string expectedPrefix)
    {
        Assert.Equal(expectedPrefix, FirmwareNameValidator.ExpectedPrefix(firmwareInfo));
    }

    [Fact]
    public void LooksMismatched_FailsOpenWhenPrefixExtractionThrows()
    {
        // firmwareInfo entirely made of >=3 trailing digits puts match.Index
        // at 0, so the vendor's own `Remove(match.Index - 1)` logic would
        // throw ArgumentOutOfRangeException here — caught, fails open.
        Assert.False(FirmwareNameValidator.LooksMismatched("firmware.img", "123"));
    }
}
