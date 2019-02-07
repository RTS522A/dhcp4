﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Helpers
{
    public class IPSubnetMask : IPAddress
    {
        public int CIDR { get; private set; }

        private void Verify()
        {
            if (!IsValidSubnetMask(this))
            {
                throw new Exception("Given mask is not valid.");
            }

            CIDR = SubnetMaskToCIDR(this);
        }

        public IPSubnetMask(long newAddress) : base(newAddress)
        {
            Verify();
        }

        public IPSubnetMask(byte[] address) : base(address)
        {
            Verify();
        }

        public IPSubnetMask(byte[] address, long scopeid) : base(address, scopeid)
        {
            Verify();
        }

        public static int SubnetMaskToCIDR(IPSubnetMask SubnetMask)
        {
            var value = BitConverter.ToUInt32(SubnetMask.GetAddressBytes(), 0);

            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }

            return count;
        }

        public static IPSubnetMask CIDRToSubnetMask(int CIDR)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidSubnetMask(IPSubnetMask SubnetMask)
        {
            var value = BitConverter.ToUInt32(SubnetMask.GetAddressBytes(), 0);
            if (value == 0)
            {
                return true;
            }

            uint total = UInt32.MaxValue;
            while (total != 0)
            {
                var ip = (uint)IPAddress.HostToNetworkOrder((int)total);

                if (ip == value)
                {
                    return true;
                }

                total = (total << 1);
            }

            return false;
        }

        public bool Equals(IPSubnetMask IPSubnetMask)
        {
            if (IPSubnetMask is null)
            {
                return false;
            }

            if (ReferenceEquals(this, IPSubnetMask))
            {
                return true;
            }

            return CIDR == CIDR;
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == GetType() && Equals(obj as IPSubnetMask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return CIDR.GetHashCode();
            }
        }

        public static bool operator ==(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            return !(obj1 == obj2);
        }

        public static bool operator <(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            return obj1.CIDR < obj2.CIDR;
        }

        public static bool operator >(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            return obj1.CIDR > obj2.CIDR;
        }

        public static bool operator <=(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            return obj1.CIDR <= obj2.CIDR;
        }

        public static bool operator >=(IPSubnetMask obj1, IPSubnetMask obj2)
        {
            return obj1.CIDR >= obj2.CIDR;
        }

        static new public IPSubnetMask Parse(string ipSubnetString)
        {
            return new IPSubnetMask(IPAddress.Parse(ipSubnetString).GetAddressBytes());
        }
    }
}