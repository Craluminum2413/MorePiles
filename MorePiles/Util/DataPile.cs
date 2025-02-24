﻿using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace MorePiles;

public class DataPile
{
    public bool Enabled { get; set; } = true;

    public string Comment { get; set; }

    [JsonProperty]
    [JsonConverter(typeof(JsonAttributesConverter))]
    public JsonObject MorePilesProperties { get; set; }

    public bool CtrlKey { get; set; } = true;

    public bool UpSolid { get; set; } = true;

    public int BulkTransferQuantity { get; set; } = 4;

    public int TransferQuantity { get; set; } = 1;

    public int StackingCapacity { get; set; } = 1;

    public AssetLocation PlaceRemoveSound { get; set; } = new AssetLocation("sounds/player/build");

    public AssetLocation StackingModel { get; set; } = new AssetLocation();

    public Dictionary<string, AssetLocation> StackingTextures { get; set; }

    public float CbScaleYByLayer { get; set; }

    public CuboidfExtended CollisionBox { get; set; } = new CuboidfExtended();

    public float? ModelItemsToStackSizeRatio { get; set; }

    public int GetShapeElementCount(ICoreAPI api)
    {
        int elementCount = 0;
        AssetLocation path = StackingModel?.Clone().WithPathAppendixOnce(".json").WithPathPrefixOnce("shapes/");
        Shape.TryGet(api, path)?.WalkElements("*", _ => elementCount++);
        return elementCount;
    }

    public GroundStoragePropertiesExtended GetProps(ICoreAPI api)
    {
        float modelItemsToStackSizeRatio = ModelItemsToStackSizeRatio == null ? (float)GetShapeElementCount(api) / (float)StackingCapacity : (float)ModelItemsToStackSizeRatio;

        return new GroundStoragePropertiesExtended()
        {
            MorePilesProperties = MorePilesProperties,

            Layout = EnumGroundStorageLayout.Stacking,
            CtrlKey = CtrlKey,
            UpSolid = UpSolid,
            BulkTransferQuantity = BulkTransferQuantity,
            TransferQuantity = TransferQuantity,
            StackingCapacity = StackingCapacity,
            PlaceRemoveSound = PlaceRemoveSound,
            StackingModel = StackingModel,
            StackingTextures = StackingTextures,
            CbScaleYByLayer = CbScaleYByLayer,
            CollisionBox = CollisionBox.Convert(),
            ModelItemsToStackSizeRatio = modelItemsToStackSizeRatio
        };
    }

    public static Dictionary<string, GroundStoragePropertiesExtended> GetPropsFromAll(ICoreAPI api, Dictionary<string, DataPile> fromDict)
    {
        Dictionary<string, GroundStoragePropertiesExtended> dict = new Dictionary<string, GroundStoragePropertiesExtended>();

        foreach (KeyValuePair<string, DataPile> elem in fromDict)
        {
            if (!elem.Value.Enabled) continue;

            dict.Add(elem.Key, elem.Value.GetProps(api).Clone());
        }

        return dict;
    }
}