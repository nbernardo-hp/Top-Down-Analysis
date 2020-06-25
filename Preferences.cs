﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TopDownAnalysis
{
    public class Preferences
    {
        //preferenceLoaded[0,*] = calc, preferenceLoaded[1,*] = cols; preferenceLoaded[*,0] = Markets, preferenceLoaded[*,1] = Sectors
        bool[,] preferenceLoaded = new bool[2, 2];
        /// <summary>
        /// Holds the calculation and column preferences for each stock type
        /// </summary>
        private Dictionary<string, Dictionary<char, Dictionary<string, bool>>> preferencesMap = new Dictionary<string, Dictionary<char, Dictionary<string, bool>>>()
        {
            ["CALC"] = new Dictionary<char, Dictionary<string, bool>>()
            {
                ['M'] = new Dictionary<string, bool>(),
                ['S'] = new Dictionary<string, bool>()
            },
            ["COL"] = new Dictionary<char, Dictionary<string, bool>>()
            {
                ['M'] = new Dictionary<string, bool>() {
                    ["NAME"] = true,
                    ["SYMBOL"] = true,
                    ["SMA200"] = true,
                    ["SMA50"] = true,
                    ["SMA20"] = true,
                    ["CHART_PATTERN"] = true,
                    ["UNEXPECTED_ITEMS"] = true,
                    ["INDIVIDUAL_RATING"] = true
                },
                ['S'] = new Dictionary<string, bool>()
                {
                    ["NAME"] = true,
                    ["SYMBOL"] = true,
                    ["SMA200"] = true,
                    ["SMA50"] = true,
                    ["SMA20"] = true,
                    ["CHART_PATTERN"] = true,
                    ["UNEXPECTED_ITEMS"] = true,
                    ["FINVIZ_RANK"] = true,
                    ["INDIVIDUAL_RATING"] = true
                }
            }//end nested Dictionary<char,Dictionary<string,bool>>
        };//end preferencesMap

        private Dictionary<string, Dictionary<string, double>> scoreMap = new Dictionary<string, Dictionary<string, double>>()
        {
            ["SMA200"] = new Dictionary<string, double>() {
                ["Up"] = 10,
                ["Up and Down"] = 5,
                ["Down"] = 0 },
            ["SMA50/20"] = new Dictionary<string, double>() {
                ["Above"] = 10,
                ["At"] = 5,
                ["Below"] = 0 },
            ["CHART_PATTERN"] = new Dictionary<string, double>() {
                ["Bull Run"] = 10,
                ["Bull Consolidation"] = 7.5,
                ["Consolidation"] = 5,
                ["Bear Consolidation"] = 2.5,
                ["Bear Run"] = 0 },
            ["UNEXPECTED_ITEMS"] = new Dictionary<string, double>() {
                ["Very Good"] = 10,
                ["Good"] = 7.5,
                ["Average"] = 5.5,
                ["Bad"] = 3.5,
                ["Very Bad"] = 1,
                ["No News"] = 5.5 }
        };//end scoreMap

        /// <summary>
        /// Returns the symbol of each stock that has been selected to be used in the overall calculation.  Returns a List to the
        /// calling function. 'M' == Markets   'S' == Sectors
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> getCalculationPreferences(char type)
        {
            var calc = from p in preferencesMap["CALC"][type]
                       where p.Value == true
                       select p.Key;
            return calc.ToList();
        }//end getCalculationPreferences

        //Get methods
        public Dictionary<char, Dictionary<string, bool>> getCalculationDictionary() { return preferencesMap["CALC"]; }
        public Dictionary<char, Dictionary<string, bool>> getColumnDictionary() { return preferencesMap["COL"]; }
        public Dictionary<string, bool> getColumnDictionary(char type) { return preferencesMap["COL"][type]; }

        /// <summary>
        /// Returns the symbol of each column that is configured to display.  Returns a List to the calling function.
        /// 'M' == Markets    'S' == Sectors
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> getColumnPreferences(char type)
        {
            var cols = from c in preferencesMap["COL"][type]
                       where c.Value == true
                       select c.Key;
            return cols.ToList();
        }//end getColumnPreferences

        /// <summary>
        /// Returns the score of the specific Stock attribute to the calling function
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double getScore(string attribute, string value)
        {
            double score = 0;

            scoreMap[attribute].TryGetValue(value, out score);

            return score;
        }//end getScore

        /// <summary>
        /// Returns the bool value for the specific preference.  If loaded returns true else false.
        /// map is either "COL" or "CALC",  type is eeither 'M' or 'S'
        /// </summary>
        /// <param name="map"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool getPreferenceLoaded(string map, char type)
        {
            int i = (map == "CALC" ? 0 : 1);
            int j = (type == 'M' ? 0 : 1);
            return preferenceLoaded[i, j];
        }//end getPreferenceLoaded

        //Set methods
        /// <summary>
        /// Sets the preferences in the preferencesMap Dictionary.  map is "CALC" or "COLS" to indicate the
        /// main entry point.  type is either 'M' or 'S' and the list provided will be associated with the
        /// either the DataGridView columns or Stock objects selected for calculation.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="type"></param>
        /// <param name="dict"></param>
        public void setPreference(string map, char type, Dictionary<string, bool> dict)
        {
            if (dict != null || dict.Count > 0)
            {
                preferencesMap[map][type] = dict;
            }
        }//end setColumnPreferences

        /// <summary>
        /// Adds the provided symbol to the preferenceMap Dictionary if it is not present.  If it is, calls setPreference
        /// and edits the preference for the provided item
        /// </summary>
        public void addStockToPreferences(char type, Stock stock)
        {
            if(!preferencesMap.ContainsKey(stock.getSymbol()))
            {
                preferencesMap["CALC"][type].Add(stock.getSymbol(), stock.getUsedInCalculation());
            } else
            {
                //TODO
            }//end if-else
        }//end addToPreferences

        /// <summary>
        /// Gets the number of columns that are displayed in the DataGridViews using the bool value in preferencesMap.
        /// Returns the integer value to the calling function
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int checkDisplayedColCount(char type)
        {
            return (from c in preferencesMap["COL"][type]
                       where c.Value == true
                       select c.Key).Count();
        }//end getDisplayedColAmount

        /// <summary>
        /// If all the columns of the display are set to false, defaults the bool value to true to prevent errors
        /// </summary>
        /// <param name="type"></param>
        private void defaultAllColsToTrue(char type)
        {
            var temp = preferencesMap["COL"][type].Keys.ToList();
            foreach(string key in temp)
            {
                preferencesMap["COL"][type][key] = true;
            }//end foreach
        }//end defaultAllColsToTrue
        public void loadPreferences()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"preferences\";
            XmlData xmlData = new XmlData();
            if(Directory.Exists(path))
            {
                if(File.Exists(Path.Combine(path, "calcpreferences.xml")))
                {
                    loadCalculationPreferences(xmlData, 'M');
                    loadCalculationPreferences(xmlData, 'S');
                }//end nested if

                if(File.Exists(Path.Combine(path, "columnpreferences.xml")))
                {
                    loadColumnPreferences(xmlData, 'M');
                    if (checkDisplayedColCount('M') < 1)
                    {
                        defaultAllColsToTrue('M');
                    }//end if

                    loadColumnPreferences(xmlData, 'S');
                    if (checkDisplayedColCount('S') < 1)
                    {
                        defaultAllColsToTrue('S');
                    }//end if
                }//end nested if
            }//end if
        }//end loadPreferences

        /// <summary>
        /// Calls the load
        /// </summary>
        public void loadCalculationPreferences(XmlData xmlData, char type)
        {
            preferencesMap["CALC"][type] = xmlData.loadCalcPreferences(type);
            preferenceLoaded[0, (type == 'M' ? 0 : 1)] = true;
        }//end loadCalculationPreferences

        /// <summary>
        /// Calls the XmlData loadloadColumnPreferences.  Loads the column preferences for the Markets and Sectors DataGridViews
        /// </summary>
        /// <param name="xmlData"></param>
        /// <param name="type"></param>
        private void loadColumnPreferences(XmlData xmlData, char type)
        {
            var cols = xmlData.loadColumnPreferences(type);
            setPreference("COL", type, cols);

            var colCount = checkDisplayedColCount(type);

            preferenceLoaded[1, (type == 'M' ? 0 : 1)] = (colCount > 0 && colCount <= preferencesMap["COL"][type].Count() ? true : false);
        }//end loadColumnPreferences

    }//end Preferences
}//end TopDownAnalysis
