//****************************************************************************************************************************************************
//* BSD 3-Clause License
//*
//* Copyright (c) 2020, Mana Battery
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
using MB.Base.MathEx.Pixel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.Patch.Advanced
{
  /// <summary>
  /// </summary>
  [Serializable]
  public readonly struct ImmutableComplexPatch : IEquatable<ImmutableComplexPatch>
  {
    /// <summary>
    /// Describes how to slice a patch
    /// </summary>
    public readonly ImmutableComplexPatchSlices Slices;

    /// <summary>
    /// Describes how to place the content on the patch (in CountX x CountY content areas)
    /// </summary>
    public readonly ImmutablePatchContentSpans ContentSpans;

    /// <summary>
    /// </summary>
    public readonly ReadOnlyArraySegment<ComplexPatchGridFlags> GridFlags;

    /// <summary>
    /// This is the size of the source patch in pixels
    /// It might be larger than the source image size.
    /// </summary>
    public readonly PxRectangleU TrimmedPatchRectanglePx;

    public ImmutableComplexPatch(ImmutableComplexPatchSlices slices, ImmutablePatchContentSpans contentSpans, in ReadOnlyArraySegment<ComplexPatchGridFlags> gridFlags)
    {
      if (!slices.IsValid)
        throw new ArgumentException("must be valid", nameof(slices));
      if (!contentSpans.IsValid)
        throw new ArgumentException("must be valid", nameof(contentSpans));
      // Verify that the grid flags contains the expected amount of entries
      if (gridFlags.Count != ((slices.CountX - 1) * (slices.CountY - 1)))
        throw new ArgumentException("must be compatible with the slices", nameof(gridFlags));

      Slices = slices;
      ContentSpans = contentSpans;
      GridFlags = gridFlags;

      var spanRangeX = Slices.GetSliceXSpanRange();
      var spanRangeY = Slices.GetSliceYSpanRange();

      TrimmedPatchRectanglePx = new PxRectangleU(spanRangeX.Start, spanRangeY.Start, spanRangeX.Length, spanRangeY.Length);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public bool IsValid => Slices.IsValid && ContentSpans.IsValid;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ImmutableComplexPatch lhs, in ImmutableComplexPatch rhs)
        => lhs.Slices == rhs.Slices && lhs.ContentSpans == rhs.ContentSpans && lhs.TrimmedPatchRectanglePx == rhs.TrimmedPatchRectanglePx &&
           IsContentEqual(lhs.GridFlags, rhs.GridFlags);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ImmutableComplexPatch lhs, in ImmutableComplexPatch rhs) => !(lhs == rhs);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ImmutableComplexPatch other && (this == other);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override int GetHashCode() => Slices.GetHashCode() ^ ContentSpans.GetHashCode() ^ TrimmedPatchRectanglePx.GetHashCode();

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public bool Equals(ImmutableComplexPatch other) => this == other;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    private static bool IsContentEqual(ReadOnlyArraySegment<ComplexPatchGridFlags> lhs, ReadOnlyArraySegment<ComplexPatchGridFlags> rhs)
    {
      var lhsSpan = lhs.AsSpan();
      var rhsSpan = rhs.AsSpan();
      if (lhsSpan.Length != rhsSpan.Length)
        return false;
      for (int i = 0; i < lhsSpan.Length; ++i)
      {
        if (lhsSpan[i] != rhsSpan[i])
          return false;
      }
      return true;
    }
  }
}

//****************************************************************************************************************************************************
