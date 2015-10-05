namespace WebLinterVsix
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidVSPackageString = "a9b5a26c-0774-412b-9b0e-8007614a9c73";
        public const string WebLinterCmdSetString = "34114698-c8d5-4a10-abc0-78d72ab6e4a4";
        public static Guid guidVSPackage = new Guid(guidVSPackageString);
        public static Guid WebLinterCmdSet = new Guid(WebLinterCmdSetString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int ContextMenuGroup = 0x1020;
        public const int LintFilesCommand = 0x0100;
        public const int CleanErrorsCommand = 0x0200;
    }
}
