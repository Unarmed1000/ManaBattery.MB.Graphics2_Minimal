#nullable enable
//****************************************************************************************************************************************************
//* BSD 3-Clause License
//*
//* Copyright (c) 2019-2024, Mana Battery
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.TextureAtlas.Basic
{
  public readonly struct AtlasNineSliceInfo : IEquatable<AtlasNineSliceInfo>
  {
    public readonly PxThicknessU NineSlicePx;
    public readonly PxThicknessU ContentMarginPx;
    public readonly AtlasNineSliceFlags Flags;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public AtlasNineSliceInfo(PxThicknessU nineSlicePx, PxThicknessU contentMarginPx, AtlasNineSliceFlags flags)
    {
      NineSlicePx = nineSlicePx;
      ContentMarginPx = contentMarginPx;
      Flags = flags;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in AtlasNineSliceInfo lhs, in AtlasNineSliceInfo rhs)
      => (lhs.NineSlicePx == rhs.NineSlicePx && lhs.ContentMarginPx == rhs.ContentMarginPx && lhs.Flags == rhs.Flags);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in AtlasNineSliceInfo lhs, in AtlasNineSliceInfo rhs) => !(lhs == rhs);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AtlasNineSliceInfo info && (this == info);

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override int GetHashCode() => NineSlicePx.GetHashCode() ^ ContentMarginPx.GetHashCode() ^ Flags.GetHashCode();

    //------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(AtlasNineSliceInfo other) => this == other;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public override string ToString() => $"NineSlice:{NineSlicePx} ContentMargin:{ContentMarginPx} Flags: {Flags}";
  }
}

//****************************************************************************************************************************************************
