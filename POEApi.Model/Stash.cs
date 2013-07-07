﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace POEApi.Model
{
    public class Stash
    {
        private List<Item> items;
        private const int tabSize = 144;
        public int NumberOfTabs { get; private set; }
        public List<Tab> Tabs { get; set; }


        internal Stash(JSONProxy.Stash proxy)
        {
            if (proxy.Items == null)
            {
                items = new List<Item>();
                NumberOfTabs = 0;
                return;
            }

            items = proxy.Items.Select(item => ItemFactory.Get(item)).ToList();
            this.NumberOfTabs = proxy.NumTabs;
            this.Tabs = ProxyMapper.GetTabs(proxy.Tabs);
        }

        public void Add(Stash stash)
        {
            items.AddRange(stash.items);
        }

        public void RefreshTab(POEModel currentModel, string currentLeague, int tabId)
        {
            string inventId = ProxyMapper.STASH + (tabId + 1).ToString();
            items.RemoveAll(i => i.inventoryId == inventId); 
            Add(currentModel.GetStash(tabId, currentLeague, true));
        }

        public List<Item> GetItemsByTab(int tabId)
        {
            ++tabId;
            return items.FindAll(i => i.inventoryId == ProxyMapper.STASH + tabId.ToString());
        }

        public List<T> Get<T>() where T : Item
        {
            return items.OfType<T>().ToList();
        }
        public List<T> Get<T>(Func<T, bool> match) where T : Item
        {
            return items.OfType<T>()
                        .Where(match)
                        .ToList();
        }

        public double GetTotalGCP()
        {
            return CurrencyHandler.GetTotalGCP(Get<Currency>());
        }

        public Dictionary<OrbType, double> GetTotalGCPDistribution()
        {
            return CurrencyHandler.GetTotalGCPDistribution(Get<Currency>());
        }

        public Dictionary<string, List<Gear>> GetDuplicateRares()
        {
            return Get<Gear>().Where(g => g.Quality == Quality.Rare && g.Name != string.Empty)
                              .GroupBy(g => g.Name)
                              .Where(g => g.Count() > 1)
                              .Select(g => g.ToList()).ToDictionary(d => d.First().Name);
        }

        public Dictionary<string, decimal> CalculateFreeSpace()
        {
            Dictionary<string, decimal> freeSpace = new Dictionary<string, decimal>();
            decimal totalSpace = NumberOfTabs * tabSize;
            freeSpace.Add("All", (items.Sum(i => (i.W * i.H)) / totalSpace) * 100);

            foreach (var group in items.GroupBy(item => item.inventoryId))
            {
                decimal sum = group.Sum(i => (i.W * i.H));
                freeSpace.Add(group.Key, (sum / tabSize) * 100);
            }

            return freeSpace;
        }
    }
}