using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAssetManagementTool {

    public class AssetBundleUpdate : IAssetBundleUpdate {

        public bool CheckUpdate(AssetBundleManifest client, AssetBundleManifest server, out List<string> deletes, out List<string> downloads) {

            deletes = new List<string>();
            downloads = new List<string>();

            if (server.Version == client.Version) {
                return false;
            }

            var @new = new Dictionary<string, string>();
            var @cur = new Dictionary<string, string>();

            foreach (var info in server.Infos) {
                @new.Add(info.AssetBundleName, info.Hash128);
            }

            foreach (var info in client.Infos) {
                @cur.Add(info.AssetBundleName, info.Hash128);
            }

            foreach (var kvp in @new) {
                if (@cur.ContainsKey(kvp.Key)) {
                    if (@cur[kvp.Key] != kvp.Value) {
                        deletes.Add(kvp.Key);
                        downloads.Add(kvp.Key);
                    }
                } else {
                    downloads.Add(kvp.Key);
                }
            }

            foreach (var kvp in @cur) {
                if (!@new.ContainsKey(kvp.Key)) {
                    deletes.Add(kvp.Key);
                }
            }

            if (deletes.Count == 0 && downloads.Count == 0) {
                return false;
            }

            return true;
        }
    }
}

