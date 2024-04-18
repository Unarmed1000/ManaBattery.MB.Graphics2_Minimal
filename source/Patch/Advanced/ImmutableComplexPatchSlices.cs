#nullable enable
//****************************************************************************************************************************************************
//* BSD 3-Clause License
//*
//* Copyright (c) 2020-2024, Mana Battery
//* All rights reserved.
//*
//* Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//*
//* 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//* 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
//*    documentation and/or other materials provided with the distribution.
//* 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this
//*    software without specific prior written permission.
//*
//* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
//* CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//* LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//* EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//****************************************************************************************************************************************************

using MB.Base.Container;
using MB.Base.MathEx;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.Patch.Advanced
{
  /// <summary>
  /// Describes the slices used for scaling the background correctly
  /// </summary>
  [Serializable]
  public readonly struct ImmutableComplexPatchSlices : IEquatable<ImmutableComplexPatchSlices>
  {
    private readonly ReadOnlyArraySegment<ImmutableComplexPatchSlice> m_slices;
    public readonly PatchFlags Flags;

    /// <summary>
    /// The number of x slices
    /// </summary>
    public readonly byte CountX;

    /// <summary>
    /// The number of y slices
    /// </summary>
    public readonly byte CountY;

    /// <summary>
    /// The number of slices that are flagged as scaleable in X
    /// </summary>
    public readonly byte CountScaleX;

    /// <summary>
    /// The number of slices that are flagged as scaleable in Y
    /// </summary>
    public readonly byte CountScaleY;

    public ImmutableComplexPatchSlices(ReadOnlyArraySegment<ImmutableComplexPatchSlice> slices, byte countX, byte countY, PatchFlags flags)
    {
      if (slices.Count != 0)
      {
        if (countX == 0)
          throw new ArgumentException($"Can not be zero", nameof(countX));
        if (countY == 0)
          throw new ArgumentException($"Can not be zero", nameof(countY));
      }
      if ((countX + countY) != slices.Count)
        throw new ArgumentException($"{nameof(slices)} entries must match the supplied {nameof(countX)}+{nameof(countY)}");


      m_slices = slices;
      CountX = countX;
      CountY = countY;
      Flags = flags;

      CountScaleX = CalcScaleSlices(m_slices.AsSpan(0, CountX));
      CountScaleY = CalcScaleSlices(m_slices.AsSpan(CountX, CountY));

      if (AsSpanX()[CountX - 1].Flags != ComplexPatchSliceFlags.None)
        throw new Exception("Invalid x-patch, the last element can not be marked with flags as makes no sense");
      if (AsSpanY()[CountY - 1].Flags != ComplexPatchSliceFlags.None)
        throw new Exception("Invalid y-patch, the last element can not be marked with flags as makes no sense");
    }

    public bool IsValid => m_slices.Count > 0;

    public ImmutableComplexPatchSlicesPatchMeshInfo CalcMeshInfo()
    {
      UInt16 numX = (UInt16)(CountX + (Flags.IsFlagged(PatchFlags.MirrorX) ? CountX - 1 : 0));
      UInt16 numY = (UInt16)(CountY + (Flags.IsFlagged(PatchFlags.MirrorY) ? CountY - 1 : 0));

      int vertexCount = numX * numY;
      int indexCount = Math.Max(numX - 1, 0) * Math.Max(numY - 1, 0) * 2 * 3;
      return new ImmutableComplexPatchSlicesPatchMeshInfo(numX, numY, vertexCount, indexCount);
    }

    /// <summary>
    /// Direct access to both the x+y spans
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<ImmutableComplexPatchSlice> AsSpan() => m_slices.AsSpan();

    /// <summary>
    /// The content spans in the x-direction
    /// </summary>
    public ReadOnlySpan<ImmutableComplexPatchSlice> AsSpanX() => m_slices.AsSpan(0, CountX);

    /// <summary>
    /// The content spans in the y-direction
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<ImmutableComplexPatchSlice> AsSpanY() => m_slices.AsSpan(CountX, CountY);

    /// <summary>
    /// The total number of content areas represented by the spans
    /// </summary>
    public UInt16 ContentAreaCount => (UInt16)(CountX * CountY);

    public SpanRangeU16 GetSliceXSpanRange()
    {
      if (CountX <= 0)
        return new SpanRangeU16();
      var span = AsSpanX();
      return SpanRangeU16.FromStartToEnd(span[0].Position, span[span.Length - 1].Position);
    }

    public SpanRangeU16 GetSliceYSpanRange()
    {
      if (CountY <= 0)
        return new SpanRangeU16();
      var span = AsSpanY();
      return SpanRangeU16.FromStartToEnd(span[0].Position, span[span.Length - 1].Position);
    }

    private static byte CalcScaleSlices(ReadOnlySpan<ImmutableComplexPatchSlice> slices)
    {
      UInt32 count = 0;
      for (int i = 0; i < slices.Length; ++i)
      {
        count += slices[i].Flags.IsFlagged(ComplexPatchSliceFlags.Scale) ? 1u : 0u;
      }
      Debug.Assert(count <= byte.MaxValue);
      return (byte)count;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ImmutableComplexPatchSlices lhs, in ImmutableComplexPatchSlices rhs)
        => ReadOnlyArraySegment.IsContentEqual(lhs.m_slices, rhs.m_slices) && lhs.Flags == rhs.Flags && lhs.CountX == rhs.CountX &&
           lhs.CountY == rhs.CountY && lhs.CountScaleX == rhs.CountScaleX && lhs.CountScaleY == rhs.CountScaleY;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ImmutableComplexPatchSlices lhs, in ImmutableComplexPatchSlices rhs) => !(lhs == rhs);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ImmutableComplexPatchSlices slices && (this == slices);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override int GetHashCode() => m_slices.GetHashCode() ^ Flags.GetHashCode() ^ CountX.GetHashCode() ^ CountY.GetHashCode() ^
                                         CountScaleX.GetHashCode() ^ CountScaleY.GetHashCode();

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public bool Equals(ImmutableComplexPatchSlices other) => this == other;
  }
}

//****************************************************************************************************************************************************
