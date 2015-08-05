#region Description
/*
ShimmerTextImporter imports Shimmer3 exported data in text format.
    It supports two modes:
        -   Raw , returns all the records at once by use of AllRecords property
        -   Async , enumerates all the records one by one , 
                Start has to be called before enumerating and Stop when finished to dispose of the file
    
TODO:

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

using FileHelpers;
#endregion

namespace Shimmer
{
    internal class ShimmerTextImporter
    {
        public ShimmerTextImporter(string filename)
        {
            this.filename = filename;
        }

        internal WimuRecord[] AllRecords
        {
            get
            { 
                this.rawEngine = new FileHelperEngine(typeof(WimuRecord));
                return this.rawEngine.ReadFile(this.filename) as WimuRecord[];
            }
        }

        internal void Start()
        {
            this.asyncEngine = new FileHelperAsyncEngine(typeof(WimuRecord));
            this.asyncEngine.BeginReadFile(this.filename);
        }

        internal IEnumerable<WimuRecord> NextRecord()
        {
            foreach (var record in this.asyncEngine)
            {
                yield return record as WimuRecord;
            }
            yield break;
        }

        public void Stop()
        {
            this.asyncEngine.Flush();
            this.asyncEngine.Close();
        }
        #region Fields
        private FileHelperAsyncEngine asyncEngine;
        private FileHelperEngine rawEngine;
        private string filename;
        #endregion
    }
}
