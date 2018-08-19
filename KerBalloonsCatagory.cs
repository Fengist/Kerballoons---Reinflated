using KSP.UI.Screens;
using RUI.Icons.Selectable;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Kerballoons
{

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KBFilter : BaseFilter
    {
        protected override string Manufacturer
        {
            get { return "KerBalloons"; }// part manufacturer in cfgs and agents files 
            set { }
        }
        protected override string categoryTitle
        {
            get { return "KerBalloons"; } // the category name 
            set { }
        }
    }    

    public abstract class BaseFilter : MonoBehaviour
    {
        private readonly List<AvailablePart> parts = new List<AvailablePart>();
        internal string category = "Filter by function";
        internal bool filter = true;
        protected abstract string Manufacturer { get; set; }
        protected abstract string categoryTitle { get; set; }

        void Awake()
        {
            parts.Clear();
            var count = PartLoader.LoadedPartsList.Count;
            for (int i = 0; i < count; ++i)
            {
                var avPart = PartLoader.LoadedPartsList[i];
                if (!avPart.partPrefab) continue;
                if (avPart.manufacturer == Manufacturer)
                {
                    parts.Add(avPart);
                }
            }
            if (parts.Count > 0)
                GameEvents.onGUIEditorToolbarReady.Add(SubCategories);
        }

        private bool EditorItemsFilter(AvailablePart avPart)
        {
            return parts.Contains(avPart);
        }

        private void SubCategories()
        {
            var icon = GetIcon(categoryTitle);
            var filter = PartCategorizer.Instance.filters.Find(f => f.button.categorydisplayName == "#autoLOC_453547");//change for 1.3.1
            PartCategorizer.AddCustomSubcategoryFilter(filter, categoryTitle, categoryTitle, icon, EditorItemsFilter);
        }

        private Icon GetIcon(string iconName)
        {
            var normIcon = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var normIconFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), iconName + "_off.png"); // icon to be present in same folder as dll
            WWW www = new WWW(normIconFile);
            www.LoadImageIntoTexture(normIcon);
            //normIcon = www.texture;
            //normIcon.LoadRawTextureData(File.ReadAllBytes(normIconFile));
            normIcon.Apply();

            var selIcon = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var selIconFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), iconName + "_on.png");// icon to be present in same folder as dll
            www = new WWW(selIconFile);
            www.LoadImageIntoTexture(selIcon);
            //selIcon = www.texture;
            //selIcon.LoadRawTextureData(File.ReadAllBytes(selIconFile));
            selIcon.Apply();
            www.Dispose();

            var icon = new Icon(iconName + "Icon", normIcon, selIcon);
            return icon;

        }
    }

}