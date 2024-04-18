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

using MB.Base;
using MB.Base.Container;
using MB.Base.MathEx.Pixel;
using System;
using System.Diagnostics;

//----------------------------------------------------------------------------------------------------------------------------------------------------

namespace MB.Graphics2.Patch.Advanced
{
  public static class ImmutableComplexPatchUtil
  {
    //private static Logger g_logger = LogManager.GetCurrentClassLogger();

    public static ImmutableComplexPatch CreateTransparentComplexPatch(ImmutableComplexPatchSlice[] sliceArray, byte sliceCountX, byte sliceCountY,
                                                                      ImmutableContentSpan[] contentSpanArray, byte spanCountX, byte spanCountY,
                                                                      PatchFlags patchFlags)
    {
      var finalSlices = new ImmutableComplexPatchSlices(ReadOnlyArraySegment.Create(sliceArray), sliceCountX, sliceCountY, patchFlags);
      var finalContentSpans = new ImmutablePatchContentSpans(ReadOnlyArraySegment.Create(contentSpanArray), spanCountX, spanCountY);
      var gridFlags = CreateGridFlags(finalSlices.AsSpanX(), finalSlices.AsSpanY());
      return new ImmutableComplexPatch(finalSlices, finalContentSpans, gridFlags);
    }

    public static ImmutableComplexPatch CreateTransparentComplexPatch(ReadOnlySpan<ImmutableComplexPatchSlice> sliceArrayX,
                                                                      ReadOnlySpan<ImmutableComplexPatchSlice> sliceArrayY,
                                                                      ReadOnlySpan<ImmutableContentSpan> contentSpanArrayX,
                                                                      ReadOnlySpan<ImmutableContentSpan> contentSpanArrayY,
                                                                      PatchFlags patchFlags)
    {
      var gridFlags = CreateGridFlags(sliceArrayX, sliceArrayY);
      return CreateComplexPatch(sliceArrayX, sliceArrayY, contentSpanArrayX, contentSpanArrayY, gridFlags, patchFlags);
    }

    public static ImmutableComplexPatch CreateComplexPatch(ImmutableComplexPatchSlice[] sliceArray, byte sliceCountX, byte sliceCountY,
                                                           ImmutableContentSpan[] contentSpanArray, byte spanCountX, byte spanCountY,
                                                           ReadOnlyArraySegment<ComplexPatchGridFlags> gridFlags, PatchFlags patchFlags)
    {
      var finalSlices = new ImmutableComplexPatchSlices(ReadOnlyArraySegment.Create(sliceArray), sliceCountX, sliceCountY, patchFlags);
      var finalContentSpans = new ImmutablePatchContentSpans(ReadOnlyArraySegment.Create(contentSpanArray), spanCountX, spanCountY);
      return new ImmutableComplexPatch(finalSlices, finalContentSpans, gridFlags);
    }

    public static ImmutableComplexPatch CreateComplexPatch(ImmutableComplexPatchSlice[] sliceArray, int sliceCountX, int sliceCountY,
                                                                   ImmutableContentSpan[] contentSpanArray, int spanCountX, int spanCountY,
                                                                   ReadOnlyArraySegment<ComplexPatchGridFlags> gridFlags,
                                                                   PatchFlags patchFlags)
    {
      return CreateComplexPatch(sliceArray, NumericCast.ToUInt8(sliceCountX), NumericCast.ToUInt8(sliceCountY),
                                contentSpanArray, NumericCast.ToUInt8(spanCountX), NumericCast.ToUInt8(spanCountY), gridFlags, patchFlags);
    }

    public static ImmutableComplexPatch CreateComplexPatch(ReadOnlySpan<ImmutableComplexPatchSlice> sliceArrayX,
                                                           ReadOnlySpan<ImmutableComplexPatchSlice> sliceArrayY,
                                                           ReadOnlySpan<ImmutableContentSpan> contentSpanArrayX,
                                                           ReadOnlySpan<ImmutableContentSpan> contentSpanArrayY,
                                                           ReadOnlyArraySegment<ComplexPatchGridFlags> gridFlags,
                                                           PatchFlags patchFlags)
    {
      // Store the slices in one array
      var slices = new ImmutableComplexPatchSlice[sliceArrayX.Length + sliceArrayY.Length];
      {
        var slicesX = slices.AsSpan(0, sliceArrayX.Length);
        var slicesY = slices.AsSpan(sliceArrayX.Length, sliceArrayY.Length);
        for (int i = 0; i < slicesX.Length; ++i)
          slicesX[i] = sliceArrayX[i];
        for (int i = 0; i < slicesY.Length; ++i)
          slicesY[i] = sliceArrayY[i];
      }

      // store the content span arrays in one array
      ImmutableContentSpan[] contentSpans = new ImmutableContentSpan[contentSpanArrayX.Length + contentSpanArrayY.Length];
      {
        var slicesX = contentSpans.AsSpan(0, contentSpanArrayX.Length);
        var slicesY = contentSpans.AsSpan(contentSpanArrayX.Length, contentSpanArrayY.Length);
        for (int i = 0; i < slicesX.Length; ++i)
          slicesX[i] = contentSpanArrayX[i];
        for (int i = 0; i < slicesY.Length; ++i)
          slicesY[i] = contentSpanArrayY[i];
      }

      return CreateComplexPatch(slices, sliceArrayX.Length, sliceArrayY.Length, contentSpans, contentSpanArrayX.Length, contentSpanArrayY.Length,
                                gridFlags, patchFlags);
    }


    public static ImmutableComplexPatch CreateExtendedTransparentComplexPatch(in PxThicknessU nineSlicePx, in PxThicknessU contentMarginPx, PxSize2D imageSize)
    {
      UInt32 width = NumericCast.ToUInt32(imageSize.Width);
      UInt32 height = NumericCast.ToUInt32(imageSize.Height);
      if (nineSlicePx.SumX > width)
        throw new Exception($"AddNineSlice.TrimMarginPx.SumX {nineSlicePx.SumX} exceeds the image width of {width}");
      if (nineSlicePx.SumY > height)
        throw new Exception($"AddNineSlice.TrimMarginPx.SumX {nineSlicePx.SumY} exceeds the image width of {height}");
      if (contentMarginPx.SumX > width)
        throw new Exception($"AddNineSlice.TrimMarginPx.SumX {contentMarginPx.SumX} exceeds the image width of {width}");
      if (contentMarginPx.SumY > height)
        throw new Exception($"AddNineSlice.TrimMarginPx.SumX {contentMarginPx.SumY} exceeds the image width of {height}");

      // Generate the slices that corrospond to a nineslice
      var slices = new ImmutableComplexPatchSlice[4 + 4];
      slices[0] = new ImmutableComplexPatchSlice(0, ComplexPatchSliceFlags.Transparent);
      slices[1] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(nineSlicePx.Left), ComplexPatchSliceFlags.Transparent | ComplexPatchSliceFlags.Scale);
      slices[2] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(imageSize.Width - nineSlicePx.Right), ComplexPatchSliceFlags.Transparent);
      slices[3] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(imageSize.Width), ComplexPatchSliceFlags.None);

      slices[4] = new ImmutableComplexPatchSlice(0, ComplexPatchSliceFlags.Transparent);
      slices[5] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(nineSlicePx.Top), ComplexPatchSliceFlags.Transparent | ComplexPatchSliceFlags.Scale);
      slices[6] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(imageSize.Height - nineSlicePx.Bottom), ComplexPatchSliceFlags.Transparent);
      slices[7] = new ImmutableComplexPatchSlice(NumericCast.ToUInt16(imageSize.Height), ComplexPatchSliceFlags.None);

      // Generate the content span corrosponding to the content margin
      var spans = new ImmutableContentSpan[1 + 1];
      spans[0] = ImmutableContentSpan.FromNearFar(NumericCast.ToUInt16(contentMarginPx.Left),
                                                  NumericCast.ToUInt16(imageSize.Width - contentMarginPx.Right));
      spans[1] = ImmutableContentSpan.FromNearFar(NumericCast.ToUInt16(contentMarginPx.Top),
                                                  NumericCast.ToUInt16(imageSize.Height - contentMarginPx.Bottom));

      Debug.Assert(spans[0].Start <= imageSize.Width);
      Debug.Assert(spans[0].End <= imageSize.Width);
      Debug.Assert(spans[1].Start <= imageSize.Height);
      Debug.Assert(spans[1].End <= imageSize.Height);

      return CreateTransparentComplexPatch(slices, 4, 4, spans, 1, 1, PatchFlags.None);
    }

    private static ReadOnlyArraySegment<ComplexPatchGridFlags> CreateGridFlags(ReadOnlySpan<ImmutableComplexPatchSlice> slicesX,
                                                                               ReadOnlySpan<ImmutableComplexPatchSlice> slicesY)
    {
      if (slicesX.Length < 2 || slicesY.Length < 2)
        throw new ArgumentException("invalid patch");

      int countX = slicesX.Length - 1;
      int countY = slicesY.Length - 1;
      var gridFlagsArray = new ComplexPatchGridFlags[countX * countY];
      var gridFlags = new ReadOnlyArraySegment<ComplexPatchGridFlags>(gridFlagsArray, 0, gridFlagsArray.Length);
      {
        // Do a ugly rough transparency grid based only on the slice information
        for (int i = 0; i < countX; ++i)
        {
          if (slicesX[i].Flags.IsFlagged(ComplexPatchSliceFlags.Transparent))
          {
            int endY = countY * countX;
            for (int y = i; y < endY; y += countX)
            {
              gridFlagsArray[y] = ComplexPatchGridFlags.Transparent;
            }
          }
        }
        for (int i = 0; i < countY; ++i)
        {
          if (slicesY[i].Flags.IsFlagged(ComplexPatchSliceFlags.Transparent))
          {
            int yOffset = countX * i;
            for (int x = 0; x < countX; ++x)
            {
              gridFlagsArray[yOffset + x] = ComplexPatchGridFlags.Transparent;
            }
          }
        }
      }

      return gridFlags;
    }
  }
}

//****************************************************************************************************************************************************
