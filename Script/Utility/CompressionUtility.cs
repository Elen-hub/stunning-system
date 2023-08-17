using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompressionUtility
{
    public abstract class Decoder
    {
        public abstract int GetValue { get; }
    }
    public static eTileCompressionType CompressionTileData(List<int> dataList, out List<int> byteArr)
    {
        eTileCompressionType compressionSelect = eTileCompressionType.None;
        int size = dataList.Count;
        byteArr = dataList;
        Debug.Log("Uncompression Count: " + dataList.Count);

        List<int> rle = CompressionRLE(dataList);
        Debug.Log("Compression RLE Algorithm Count: " + rle.Count);
        if(size >= rle.Count)
        {
            compressionSelect = eTileCompressionType.RLE;
            byteArr = rle;
        }

        return compressionSelect;
    }
    static List<int> CompressionRLE(IEnumerable<int> compression)
    {
        List<int> dataList = new List<int>();
        var enumerator = compression.GetEnumerator();
        enumerator.MoveNext();
        int value = enumerator.Current;
        int count = 1;
        while (enumerator.MoveNext())
        {
            if (value == enumerator.Current)
            {
                ++count;
                continue;
            }
            dataList.Add(value);
            dataList.Add(count);
            value = enumerator.Current;
            count = 1;
        }
        dataList.Add(value);
        dataList.Add(count);
        return dataList;
    }
    public class RLEDecoder : Decoder
    {
        SaveController _saveController;
        int _count = 0;
        int _currentValue = 0;
        public override int GetValue
        {
            get
            {
                if (_count != 0)
                {
                    --_count;
                    return _currentValue;
                }
                _currentValue = _saveController.GetInt32();
                _count = _saveController.GetInt32();
                return GetValue;
            }
        }
        public RLEDecoder(SaveController saveController)
        {
            _saveController = saveController;
        }
    }
}
