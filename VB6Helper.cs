using Microsoft.Win32;

namespace Inedo.BuildMasterExtensions.VB6
{
    internal sealed class VB6Helper
    {
        public static VB6Helper Create() { return new VB6Helper(); }

        public string GetVB6ProductDir()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\6.0\Setup\Microsoft Visual Basic"))
            {
                if (key == null)
                    return null;

                var val = key.GetValue("ProductDir");
                if (val == null)
                    return null;

                return val.ToString();
            }
        }
    }
}
