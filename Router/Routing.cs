﻿using PacketDotNet;
using System;
using System.Net.NetworkInformation;
using Router.ARP;

namespace Router
{
    static class Routing
    {
        public static void OnReceived(IPv4Packet IPPacket)
        {
            IPPacket.TimeToLive--;
            if (IPPacket.TimeToLive <= 0)
            {
                Console.WriteLine("TimeToLive reached 0, dropping packet.");
                return;
            }

            var RoutingEntry = RoutingTable.Instance.Lookup(IPPacket.DestinationAddress);
            if (RoutingEntry == null)
            {
                Console.WriteLine("No RoutingEntry for {0}.", IPPacket.DestinationAddress);
                return;
            }

            if (!RoutingEntry.HasInterface)
            {
                Console.WriteLine("No Interface after RoutingTable Lookup for {0}.", IPPacket.DestinationAddress);
                return;
            }

            PhysicalAddress DestionationMac = ARPMiddleware.Lookup(RoutingEntry.NextHopIP, RoutingEntry.Interface);
            if (DestionationMac == null)
            {
                Console.WriteLine("No DestionationMac after ARP Lookup for {0}.", RoutingEntry.NextHopIP);
                return;
            }

            // Send
            var ethernetPacket = new EthernetPacket(RoutingEntry.Interface.PhysicalAddress, DestionationMac, EthernetPacketType.IpV4);
            ethernetPacket.PayloadData = IPPacket.Bytes;
            RoutingEntry.Interface.SendPacket(ethernetPacket.Bytes);
        }
    }
}
