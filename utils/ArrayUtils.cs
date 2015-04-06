using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StreetDirectionViewer {
  class ArrayUtils {

    public static IEnumerable<T> Array16Enumerable<T>(Array16<T> array) {

      // Surely there is a better way to iterate over all the segments?

      FieldInfo unusedItemsField = array.GetType().GetField("m_unusedItems", BindingFlags.NonPublic | BindingFlags.Instance);
      ushort[] unusedItems = (ushort[])unusedItemsField.GetValue(array);

      FieldInfo unusedCountField = array.GetType().GetField("m_unusedCount", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance);
      uint unusedCount = (uint)unusedCountField.GetValue(array);

      HashSet<uint> unused = new HashSet<uint>();
      for (uint i = 0; i < unusedCount; i++) {
        unused.Add(unusedItems[i]);
      }

      for (uint i = 0; i < array.m_size; i++) {
        if (!unused.Contains(i)) {
          yield return array.m_buffer[i];
        }
      }
    }
  }
}
