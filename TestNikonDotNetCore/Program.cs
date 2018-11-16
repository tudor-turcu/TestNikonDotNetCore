using System;
using Nikon;

namespace TestNikonDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            NikonManager nikonManager = new NikonManager("Type0004.md3"); // D7000

            try
            {
                nikonManager.DeviceAdded += OnDeviceAdded;

                Console.WriteLine("Press any key to continue..");
                Console.ReadLine();
            }
            finally
            {
                nikonManager.Shutdown();
            }
        }

        private static void OnDeviceAdded(NikonManager sender, NikonDevice device)
        {
            Console.WriteLine("Nikon camera added...");

            try
            {
                int batteryLevel = device.GetInteger(eNkMAIDCapability.kNkMAIDCapability_BatteryLevel);
                Console.WriteLine($"Battery level: {batteryLevel}");

                PrintCapabilities(device);
            }
            catch (NikonException ex)
            {
                Console.WriteLine($"Error getting the battery level: {ex.Message}, code: {ex.ErrorCode}");
            }
        }

        private static void PrintCapabilities(NikonDevice device)
        {
            Console.WriteLine("Capabilities:");

            // Get 'info' struct for each supported capability
            NkMAIDCapInfo[] caps = device.GetCapabilityInfo();

            // Iterate through all supported capabilities
            foreach (NkMAIDCapInfo cap in caps)
            {
                // Print ID, description and type
                Console.WriteLine($"{"Id",-14}: {cap.ulID.ToString()}");
                Console.WriteLine($"{"Description",-14}: {cap.GetDescription()}");
                Console.WriteLine($"{"Type",-14}: {cap.ulType.ToString()}");

                // Try to get the capability value
                string value = null;

                // First, check if the capability is readable
                if (cap.CanGet())
                {
                    // Choose which 'Get' function to use, depending on the type
                    switch (cap.ulType)
                    {
                        case eNkMAIDCapType.kNkMAIDCapType_Unsigned:
                            value = device.GetUnsigned(cap.ulID).ToString();
                            break;

                        case eNkMAIDCapType.kNkMAIDCapType_Integer:
                            value = device.GetInteger(cap.ulID).ToString();
                            break;

                        case eNkMAIDCapType.kNkMAIDCapType_String:
                            value = device.GetString(cap.ulID);
                            break;

                        case eNkMAIDCapType.kNkMAIDCapType_Boolean:
                            value = device.GetBoolean(cap.ulID).ToString();
                            break;

                            // Note: There are more types - adding the rest is left
                            //       as an exercise for the reader.
                    }
                }

                // Print the value
                if (value != null)
                {
                    Console.WriteLine($"{"Value",-14}: {value}");
                }

                // Print spacing between capabilities
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
