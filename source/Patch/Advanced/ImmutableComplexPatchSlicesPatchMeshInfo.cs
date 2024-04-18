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

using System;
using System.Diagnostics.CodeAnalysis;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.Patch.Advanced
{
  public readonly struct ImmutableComplexPatchSlicesPatchMeshInfo : IEquatable<ImmutableComplexPatchSlicesPatchMeshInfo>
  {
    public readonly UInt16 VertexCountX;
    public readonly UInt16 VertexCountY;
    public readonly int VertexCount;
    public readonly int IndexCount;

    public ImmutableComplexPatchSlicesPatchMeshInfo(UInt16 vertexCountX, UInt16 vertexCountY, int vertexCount, int indexCount)
    {
      VertexCountX = vertexCountX;
      VertexCountY = vertexCountY;
      VertexCount = vertexCount;
      IndexCount = indexCount;
    }

    public static bool operator ==(in ImmutableComplexPatchSlicesPatchMeshInfo lhs, in ImmutableComplexPatchSlicesPatchMeshInfo rhs)
      => lhs.VertexCountX == rhs.VertexCountX && lhs.VertexCountY == rhs.VertexCountY && lhs.VertexCount == rhs.VertexCount &&
         lhs.IndexCount == rhs.IndexCount;

    public static bool operator !=(in ImmutableComplexPatchSlicesPatchMeshInfo lhs, in ImmutableComplexPatchSlicesPatchMeshInfo rhs)
      => !(lhs == rhs);

    public bool Equals(ImmutableComplexPatchSlicesPatchMeshInfo other) => this == other;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ImmutableComplexPatchSlicesPatchMeshInfo info && this == info;

    public override int GetHashCode()
      => VertexCountX.GetHashCode() ^ VertexCountY.GetHashCode() ^ VertexCount.GetHashCode() ^ IndexCount.GetHashCode();
  }
}

//****************************************************************************************************************************************************
