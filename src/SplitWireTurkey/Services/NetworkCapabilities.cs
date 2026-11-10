namespace SplitWireTurkey.Services;

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public static class NetworkCapabilities
{
    public static bool HasGlobalIPv6()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                if (iface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    iface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                IPInterfaceProperties ipProps;
                try
                {
                    ipProps = iface.GetIPProperties();
                }
                catch
                {
                    continue;
                }

                if (ipProps == null)
                {
                    continue;
                }

                bool hasIpv6Gateway = false;
                foreach (var gateway in ipProps.GatewayAddresses)
                {
                    if (gateway?.Address != null &&
                        gateway.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                        !IPAddress.IPv6Any.Equals(gateway.Address) &&
                        !IPAddress.IPv6None.Equals(gateway.Address))
                    {
                        hasIpv6Gateway = true;
                        break;
                    }
                }

                if (!hasIpv6Gateway)
                {
                    continue;
                }

                foreach (var unicast in ipProps.UnicastAddresses)
                {
                    var address = unicast?.Address;
                    if (address == null || address.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }

                    if (address.IsIPv6LinkLocal ||
                        address.IsIPv6SiteLocal ||
                        address.IsIPv6Multicast ||
                        IPAddress.IsLoopback(address) ||
                        address.Equals(IPAddress.IPv6Any) ||
                        address.Equals(IPAddress.IPv6None))
                    {
                        continue;
                    }

                    byte[] bytes = address.GetAddressBytes();
                    if ((bytes[0] & 0xFE) == 0xFC)
                    {
                        continue;
                    }

                    if (bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x00 && bytes[3] == 0x00)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
