//****************************************************************************************************************************************************
//* BSD 3-Clause License
//*
//* Copyright (c) 2019, Mana Battery
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

using MB.Base.MathEx.Pixel;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.TextureAtlas.Basic
{
  public readonly struct AtlasTextureInfo : IEquatable<AtlasTextureInfo>
  {
    public readonly PxPoint2 OffsetPx;
    public readonly PxExtent2D ExtentPx;
    public readonly PxRectangleU TrimmedRectPx;
    public readonly PxThicknessU TrimMarginPx;
    public readonly UInt32 Dpi;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public AtlasTextureInfo(PxRectangleU trimmedRectPx, PxThicknessU trimMarginPx, UInt32 dpi)
    {
      Debug.Assert(trimmedRectPx.X <= (UInt32)int.MaxValue);
      Debug.Assert(trimMarginPx.Left <= (UInt32)int.MaxValue);
      Debug.Assert(trimmedRectPx.Y <= (UInt32)int.MaxValue);
      Debug.Assert(trimMarginPx.Top <= (UInt32)int.MaxValue);

      OffsetPx = new PxPoint2((int)trimmedRectPx.X - (int)trimMarginPx.Left, (int)trimmedRectPx.Y - (int)trimMarginPx.Top);
      ExtentPx = trimmedRectPx.Extent + trimMarginPx.Sum;
      TrimmedRectPx = trimmedRectPx;
      TrimMarginPx = trimMarginPx;
      Dpi = dpi;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(AtlasTextureInfo lhs, AtlasTextureInfo rhs)
      => lhs.OffsetPx == rhs.OffsetPx && lhs.ExtentPx == rhs.ExtentPx && lhs.TrimmedRectPx == rhs.TrimmedRectPx &&
         lhs.TrimMarginPx == rhs.TrimMarginPx && lhs.Dpi == rhs.Dpi;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(AtlasTextureInfo lhs, AtlasTextureInfo rhs) => !(lhs == rhs);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AtlasTextureInfo other && (this == other);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override int GetHashCode() => OffsetPx.GetHashCode() ^ ExtentPx.GetHashCode() ^ TrimmedRectPx.GetHashCode() ^ TrimMarginPx.GetHashCode();

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(AtlasTextureInfo other) => this == other;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override string ToString()
      => $"OffsetPx:{OffsetPx} ExtentPx:{ExtentPx} TrimmedRectPx:{TrimmedRectPx} TrimMarginPx:{TrimMarginPx} Dp:{Dpi}";
  }
}

//****************************************************************************************************************************************************
