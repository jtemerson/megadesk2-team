﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MegaDesk_3_JTEmerson
{
    public class DeskQuote
    {
        public string CustomerName { get; set; }
        public DateTime QuoteDate;
        public Desk Desk { get; set; }
        public int RushDays { get; set; }
        public decimal QuoteAmount{ get; set; }
        private int[,] RushDayPrices;

        private const int BASE_PRICE = 200;
        private const int SIZE = 1000;
        private const int PRICE_PER_DRAWER = 50;
        private const int PRICE_PER_SURFACEAREA = 1;
        private const int RUSH1 = 3;
        private const int RUSH2 = 5;
        private const int RUSH3 = 7;
        private const int RUSH = 2000;

        public decimal CreateQuoteTotal(DeskQuote deskQuote)
        {
            //define required area vars
            int surfaceAreaCost;
            int surfaceArea = deskQuote.Desk.Depth * deskQuote.Desk.Width;
            int costOfDrawers;
            int materialCost;
            int rushOrderCost = 0;

            //calculate surface area cost
            if (surfaceArea > SIZE)
            {
                surfaceAreaCost = (surfaceArea - SIZE) * PRICE_PER_SURFACEAREA;
            }
            else
            {
                surfaceAreaCost = 0;
            }

            //calculate drawer cost
            costOfDrawers = deskQuote.Desk.NumberOfDrawers * PRICE_PER_DRAWER;

            //calculate material cost
            switch (deskQuote.Desk.Material)
            {
                case Material.laminate:
                    materialCost = 100;
                    break;
                case Material.oak:
                    materialCost = 200;
                    break;
                case Material.pine:
                    materialCost = 50;
                    break;
                case Material.veneer:
                    materialCost = 125;
                    break;
                case Material.rosewood:
                    materialCost = 300;
                    break;
                default:
                    materialCost = -1; // error flag
                    break;
            }

            //calculate rush order
            
            if (deskQuote.RushDays == RUSH1)
            {
                RushDayPrices = GetRushOrderPrices();

                if (surfaceArea < SIZE)
                {
                    rushOrderCost = RushDayPrices[0, 0];
                }
                else if (surfaceArea <= RUSH)
                {
                    rushOrderCost = RushDayPrices[0, 1];
                }
                else
                {
                    rushOrderCost = RushDayPrices[0, 2];
                }
            }
            if (deskQuote.RushDays == RUSH2)
            {
                if (surfaceArea < SIZE)
                {
                    rushOrderCost = RushDayPrices[1, 0];
                }
                else if (surfaceArea <= RUSH)
                {
                    rushOrderCost = RushDayPrices[1, 1];
                }
                else
                {
                    rushOrderCost = RushDayPrices[1, 2];
                }
            }
            if (deskQuote.RushDays == RUSH3)
            {
                if (surfaceArea < SIZE)
                {
                    rushOrderCost = RushDayPrices[2, 0];
                }
                else if (surfaceArea <= RUSH)
                {
                    rushOrderCost = RushDayPrices[2, 1];
                }
                else
                {
                    rushOrderCost = RushDayPrices[2, 2];
                }
            }

            return BASE_PRICE + surfaceAreaCost + costOfDrawers + materialCost + rushOrderCost;
        }

        private int[,] GetRushOrderPrices()
        {
            int[,] rushPrices = new int[3, 3];
            int i = 0, j = 0;

            try
            {
                string[] prices = File.ReadAllLines("rushOrderPrices.txt");

                foreach (var price in prices)
                {
                    int value;
                    if (Int32.TryParse(price, out value))
                    {
                        rushPrices[i, j] = value;
                        j++;
                        if (j > 2)
                        {
                            i++;
                            j = 0;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Error loading Rush Order Pricing"); // Tell user about the error
            }
            return rushPrices;
        }
    }
}