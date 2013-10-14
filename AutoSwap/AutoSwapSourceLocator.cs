using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Samples.Debugging.CorSymbolStore;

namespace AutoSwap {
    public class AutoSwapSourceLocator {
        public string FindSourcePath(Type type) {
            var reader = GetSymbolReader(type.Assembly);
            if (reader == null)
                return null;
            
            var symbolMethod = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                   .Select(m => reader.GetMethod(new SymbolToken(m.MetadataToken)))
                                   .FirstOrDefault(s => s != null);

            if (symbolMethod == null)
                return null;

            var count = symbolMethod.SequencePointCount;

            var offsets = new int[count];
            var docs = new ISymbolDocument[count];
            var startColumns = new int[count];
            var endColumns = new int[count];
            var startRows = new int[count];
            var endRows = new int[count];
            symbolMethod.GetSequencePoints(offsets, docs, startRows, startColumns, endRows, endColumns);

            var paths = docs.Select(d => d.URL).Distinct();
            return paths.Single();
        }

        private ISymbolReader GetSymbolReader(Assembly assembly) {
            const int E_PDB_NO_DEBUG_INFO = unchecked((int)0x806D0014);
            try {
                return SymbolAccess.GetReaderForFile(assembly.Location);
            }
            catch (COMException ex) {
                if (ex.ErrorCode == E_PDB_NO_DEBUG_INFO)
                    return null;

                throw;
            }
        }
    }
}