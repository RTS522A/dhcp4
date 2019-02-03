﻿using PacketDotNet;
using Router.RIP;
using SharpPcap;
using System;

namespace Router
{
    class Handler
    {
        private Interface Interface;

        private Type PacketType;
        private Action PacketHandler;

        public Handler(RawCapture RawCapture, Interface Interface)
        {
            this.Interface = Interface;

            var packet = (EthernetPacket)PacketDotNet.Packet.ParsePacket(RawCapture.LinkLayerType, RawCapture.Data);
            if (packet == null)
            {
                throw new Exception("Packet is not Ethernet Packet.");
            }

            // My packet?
            if(Equals(packet.SourceHwAddress, Interface.PhysicalAddress))
            {
                PacketType = null;
                return;
            }

            ARPPacket arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
            if (arpPacket != null)
            {
                PacketType = typeof(ARPPacket);
                PacketHandler = () => ARP(arpPacket);
                return;
            }

            RIPPacket ripPacket = Protocols.RIP.Parse(packet);
            if (ripPacket != null)
            {
                PacketType = typeof(RIPPacket);
                PacketHandler = () => RIP(ripPacket);
                return;
            }

            IPv4Packet ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet));
            if (ipPacket != null)
            {
                PacketType = typeof(IPv4Packet);
                PacketHandler = () => IP(ipPacket);
                return;
            }

            // Other packets ignore
            PacketType = null;
            return;
        }

        public bool Exists()
        {
            return PacketHandler != null;
        }

        public bool CheckType(Type Type)
        {
            return Equals(Type, PacketType);
        }

        public void Execute()
        {
            if(PacketType != null)
            {
                PacketHandler();
            }
        }

        public void RIP(RIPPacket RIPPacket)
        {
            Console.WriteLine("Got RIP.");
        }

        public void ARP(ARPPacket ARPPacket)
        {
            Console.WriteLine("Got ARP.");
        }

        public void IP(IPv4Packet IPv4Packet)
        {
            Console.WriteLine("Got IPV4.");
        }
    }
}