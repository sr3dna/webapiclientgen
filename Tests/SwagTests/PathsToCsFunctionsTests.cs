using Fonlow.OpenApi.ClientTypes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.IO;
using Xunit;
using Fonlow.OpenApiClientGen.Cs;

namespace SwagTests
{
    public class ToCsFunctions
    {
        static OpenApiDocument ReadJson(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new OpenApiStreamReader().Read(stream, out var diagnostic);
            }
        }

        static string TranslateJsonToCode(string filePath, Settings mySettings=null)
        {
            OpenApiDocument doc = ReadJson(filePath);

            Settings settings = mySettings ?? new Settings()
            {
                ClientNamespace = "MyNS",
                PathPrefixToRemove="/api",
                ContainerClassName="Misc",
                ContainerNameStrategy= ContainerNameStrategy.Tags,
                GenerateBothAsyncAndSync=true
            };
            var gen = new ControllersClientApiGen(settings);
            gen.CreateCodeDom(doc.Paths, doc.Components);
            return gen.WriteToText();
        }

        static string ReadFromResults(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        [Fact]
        public void TestValuesPaths()
        {
            var s = TranslateJsonToCode("SwagMock\\ValuesPaths.json");
            Assert.Equal(ReadFromResults("Results\\ValuesPaths.txt"), s);
        }

        [Fact]
        public void TestSimplePet()
        {
            string expected = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyNS
{
	
	
	public class Pet
	{
		
		/// <summary>
		/// The name given to a pet
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Type of a pet
		/// </summary>
		public string PetType { get; set; }
	}
}
";
            var s = TranslateJsonToCode("SwagMock\\SimplePet.json");
            Assert.Equal(expected, s);
        }

        [Fact]
        public void TestPet()
        {
            var s = TranslateJsonToCode("SwagMock\\pet.yaml");
            Assert.Equal(ReadFromResults("Results\\Pet.txt"), s);
        }

        [Fact]
        public void TestPetWithPathAsContainerName()
        {
            var s = TranslateJsonToCode("SwagMock\\pet.yaml", new Settings()
            {
                ClientNamespace = "MyNS",
                ContainerClassName = "Misc",
                ActionNameStrategy = ActionNameStrategy.MethodQueryParameters,
                ContainerNameStrategy = ContainerNameStrategy.Path,
                GenerateBothAsyncAndSync = false
            });
            Assert.Equal(ReadFromResults("Results\\PetPathAsContainer.txt"), s);
        }

        [Fact]
        public void TestPetWithGodContainerAndPathAction()
        {
            var s = TranslateJsonToCode("SwagMock\\pet.yaml", new Settings()
            {
                ClientNamespace = "MyNS",
                ActionNameStrategy = ActionNameStrategy.PathMethodQueryParameters,
                ContainerNameStrategy = ContainerNameStrategy.None,
                GenerateBothAsyncAndSync = false
            });
            Assert.Equal(ReadFromResults("Results\\PetGodClass.txt"), s);
        }

        [Fact]
        public void TestPetFindByStatus()
        {
            var s = TranslateJsonToCode("SwagMock\\PetFindByStatus.json", new Settings()
            {
                ClientNamespace = "MyNS",
                PathPrefixToRemove = "/api",
                ContainerClassName = "Misc",
                SuffixOfContainerName="",
                GenerateBothAsyncAndSync = true
            });
            Assert.Equal(ReadFromResults("Results\\PetFindByStatus.txt"), s);
        }

        [Fact]
        public void TestPetDelete()
        {
            var s = TranslateJsonToCode("SwagMock\\PetDelete.json");
            Assert.Equal(ReadFromResults("Results\\PetDelete.txt"), s);
        }

        [Fact]
        public void TestPetTypes()
        {
            var s = TranslateJsonToCode("SwagMock\\PetTypes.json");
            Assert.Equal(ReadFromResults("Results\\PetTypes.txt"), s);
        }




    }
}