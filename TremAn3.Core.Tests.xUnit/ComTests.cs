using System;
using System.Collections.Generic;
using TremAn3.Core;
using Xunit;
using System.Linq;

namespace TremAn3.Core.Tests.xUnit
{
    public class ComTests
    {
        CenterOfMotionAlgorithm cOM1 = new CenterOfMotionAlgorithm(256, 256, 30);
        CenterOfMotionAlgorithm cOM2 = new CenterOfMotionAlgorithm(256, 256, 30);
        public ComTests()
        {
            cOM1.Frame1 = new byte[65536];cOM1.Frame2 = new byte[65536];
            cOM2.Frame1 = new byte[65536]; cOM2.Frame2 = new byte[65536];
            for (int i = 0; i < 65536; i++)
            {
                cOM1.Frame1[i] = 0;
                cOM1.Frame2[i] = 0;
                if (i < 32768)
                {
                    if ((i / 128) % 2 == 0)
                    {
                        cOM2.Frame1[i] = 0;
                        cOM2.Frame2[i] = 0;
                    }
                    else
                    {
                        cOM2.Frame1[i] = 1;
                        cOM2.Frame2[i] = 0;
                    }
                }
                else
                {
                    if ((i / 128) % 2 == 0)
                    {
                        cOM2.Frame1[i] = 1;
                        cOM2.Frame2[i] = 0;
                    }
                    else
                    {
                        cOM2.Frame1[i] = 0;
                        cOM2.Frame2[i] = 0;
                    }
                }
            }
        }

        [Fact]
        public void GetComFromCurrentFrames_SameFrames_SameResult()
        {
            cOM1.GetComFromCurrentFrames();
            Assert.Equal(127.5, cOM1.listComX[0]);//Matlab vyhodí taky 128.. ale má jiné indexování
        }
        [Fact]
        public void GetComFromCurrentFrames_DifferentFrames_SameResult()
        {
            cOM2.GetComFromCurrentFrames();
            Assert.Equal(127.5, cOM2.listComX[0]);//Matlab vyhodí 128,5 ale má jiné indexování
        }
    }
}
