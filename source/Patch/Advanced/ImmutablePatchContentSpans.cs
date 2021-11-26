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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.Patch.Advanced
{
  /// <summary>
  /// Represents the content spans
  /// The areas are ordred from left to right, top to bottom (0,0 is top, left. x increases to the right, y increases downwards)
  /// </summary>
  [Serializable]
  public readonly struct ImmutablePatchContentSpans : IEquatable<ImmutablePatchContentSpans>
  {
    private readonly ReadOnlyArraySegment<ImmutableContentSpan> m_spans;

    /// <summary>
    /// The number of x-content spans
    /// </summary>
    public readonly byte CountX;

    /// <summary>
    /// The number of y-content spans
    /// </summary>
    public readonly byte CountY;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutablePatchContentSpans(ReadOnlyArraySegment<ImmutableContentSpan> spans, byte countX, byte countY)
    {
      if (spans.Count != 0)
      {
        if (countX == 0)
          throw new ArgumentException($"Can not be zero", nameof(countX));
        if (countY == 0)
          throw new ArgumentException($"Can not be zero", nameof(countY));
      }
      if ((countX + countY) != spans.Count)
        throw new ArgumentException($"{nameof(spans)} entries must match the supplied {nameof(countX)}+{nameof(countY)}");

      m_spans = spans;
      CountX = countX;
      CountY = countY;
    }

    public bool IsValid => m_spans.Count > 0;

    /// <summary>
    /// Get direct access to both the x+y spans
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<ImmutableContentSpan> AsSpan() => m_spans.AsSpan();

    /// <summary>
    /// The content spans in the x-direction
    /// </summary>
    public ReadOnlySpan<ImmutableContentSpan> AsSpanX() => m_spans.AsSpan(0, CountX);

    /// The content spans in the y-direction
    public ReadOnlySpan<ImmutableContentSpan> AsSpanY() => m_spans.AsSpan(CountX, CountY);

    /// <summary>
    /// The total number of content areas represented by the spans
    /// </summary>
    public UInt16 ContentAreaCount => (UInt16)(CountX * CountY);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ImmutablePatchContentSpans lhs, in ImmutablePatchContentSpans rhs)
        => ReadOnlyArraySegment.IsContentEqual(lhs.m_spans, rhs.m_spans) && lhs.CountX == rhs.CountX && lhs.CountY == rhs.CountY;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ImmutablePatchContentSpans lhs, in ImmutablePatchContentSpans rhs) => !(lhs == rhs);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ImmutablePatchContentSpans other && (this == other);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override int GetHashCode() => m_spans.GetHashCode() ^ CountX.GetHashCode() ^ CountY.GetHashCode();

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public bool Equals(ImmutablePatchContentSpans other) => this == other;
  }
}

//****************************************************************************************************************************************************
