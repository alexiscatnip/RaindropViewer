﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Disk;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TestTools;
using UnityScripts.Disk;

namespace Raindrop.Tests.DiskAndCache
{
    public class DiskTests
    {
        [Test]
        public void ImportantPaths_Platforms_Print()
        {
            Debug.Log("Application.persistentDataPath, usually the internal SD for android"
                      + Application.persistentDataPath);
            // // should be  /storage/emulated/0/Android/data/com.UnityTestRunner.UnityTestRunner/files/Pictures/
            // var target = Path.Combine(Application.persistentDataPath, "ImportantPaths_Platforms_IsWritable.txt"); 
            // File.WriteAllLines(target, new List<string> {"success!"});


            Debug.Log("Application.dataPath, usually not usable for cache " + Application.dataPath);
            // // Debug.Log("GetAndroidExternalFilesDir internal"+ Disk.DirectoryHelpers.GetAndroidExternalFilesDir(true));
            // // should be  /storage/6106-8710/Android/data/com.UnityTestRunner.UnityTestRunner/files
            // target = Path.Combine(Application.dataPath, "ImportantPaths_Platforms_IsWritable.txt");
            // File.WriteAllLines(target, new List<string> {"success!"});
            // Assert.Fail();


            Debug.Log("GetInternalCacheDir " + Disk.DirectoryHelpers.GetInternalStorageDir());
            // //should be  /storage/emulated/0/Android/data/com.UnityTestRunner.UnityTestRunner/files/Pictures/
            // target = Path.Combine(DirectoryHelpers.GetInternalCacheDir(), "ImportantPaths_Platforms_IsWritable.txt");
            // File.WriteAllLines(target, new List<string> {"success!"});
        }

        [Test]
        public void InternalCachePath_Platforms_Writeable()
        {
            Debug.Log("GetInternalCacheDir " + Disk.DirectoryHelpers.GetInternalStorageDir());
            //should be  /storage/emulated/0/Android/data/com.UnityTestRunner.UnityTestRunner/files/Pictures/
            var target = Path.Combine(DirectoryHelpers.GetInternalStorageDir(), "ImportantPaths_Platforms_IsWritable.txt");
            File.WriteAllLines(target, new List<string> {"success!"});
        }

        [UnityTest]
        // Test: StaticFilesCopier will restore missing static file in folder.
        public IEnumerator StaticAssetsFolder_MissingFile_IsRestored()
        {
            //1. delete grids.xml
            string GridsXmlFile = Path.Combine(
                DirectoryHelpers.GetInternalStorageDir(),
                "grids.xml");
            File.Delete(GridsXmlFile);
            Assert.False(File.Exists(GridsXmlFile),
                "delete grids.xml failed : " + GridsXmlFile);

            //1b. delete EULA file
            string EulaFile = Path.Combine(
                DirectoryHelpers.GetInternalStorageDir(),
                "RD_Eula.txt");
            File.Delete(EulaFile);
            Assert.False(File.Exists(EulaFile),
                "delete RD_Eula.txt failed : " + EulaFile);
            
            //2. do the startup copy.
            LMV_ExtendedTests.Helpers.DoStartupCopy();
            
            //3. grids.xml is expected to be copied
            Assert.True(File.Exists(GridsXmlFile),
                "failed to copy grids.xml from Streaming assets to this location: " + GridsXmlFile);
            
            //3b. RD_Eula.txt is expected to be copied
            Assert.True(File.Exists(EulaFile),
                "failed to copy RD_Eula.txt from Streaming assets to this location: " + EulaFile);

            yield break;
        }
        
        
        [UnityTest]
        // Test: StaticFilesCopier will restore changed static file in folder.
        public IEnumerator StaticAssetsFolder_ChangedFile_IsRestored()
        {
            //1. change grids.xml
            string GridsXmlFile = Path.Combine(
                DirectoryHelpers.GetInternalStorageDir(),
                "grids.xml");
            File.WriteAllBytes(GridsXmlFile, new byte[]{0x01});

            //2. do the startup copy.
            LMV_ExtendedTests.Helpers.DoStartupCopy();
            
            //3. grids.xml is expected to be copied
            Assert.True(File.Exists(GridsXmlFile),
                "failed to copy grids.xml from Streaming assets to this location: " + GridsXmlFile);
            
            yield break;
        }
    }
}