﻿using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPTable
    {
        internal static RIPTable Instance { get; } = new RIPTable();

        private List<RIPEntry> Entries = new List<RIPEntry>();

        public void Add(RIPEntry Entry)
        {
            Entries.Add(Entry);
        }

        public List<RIPEntry> Find(Interface Interface)
        {
            return Entries.FindAll(Entry => Entry.Interface == Interface && !Entry.ToBeRemoved);
        }

        public RIPEntry Find(Interface Interface, IPNetwork IPNetwork)
        {
            return Entries.Find(Entry => Entry.Interface == Interface && Entry.IPNetwork == IPNetwork && !Entry.ToBeRemoved);
        }

        public void Flush()
        {
            Entries = new List<RIPEntry>();
        }

        public List<RIPEntry> BestEntries()
        {
            return GetEntries().GroupBy(
                Entry => Entry.IPNetwork,
                Entry => Entry,
                (BaseIPNetwork, Routes) => Routes.BestRoute()).ToList();
        }

        public List<RIPEntry> GetEntries()
        {
            return Entries.FindAll(Entry => !Entry.ToBeRemoved);
        }

        public void GarbageCollector()
        {
            Entries.RemoveAll(Entry => Entry.ToBeRemoved);
        }
    }

    static class RIPListExtensions
    {
        public static RIPEntry BestRoute(this IEnumerable<RIPEntry> Entries)
        {
            return Entries.OrderBy(Entry => Entry.Metric).First();
        }
    }
}
