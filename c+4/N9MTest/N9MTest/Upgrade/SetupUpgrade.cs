﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace N9MTest.Upgrade
{
//    [SetUpFixture]
    class SetupUpgrade
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            Console.WriteLine("[{0}]RunBeforeAnyTests", this.GetType().Name);
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            Console.WriteLine("[{0}]RunAfterAnyTests", this.GetType().Name);
        }
    }
}
