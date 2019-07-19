using System.Collections;
using System.Collections.Generic;

namespace LGamekit.LegacyAssetManagementSystem {

    public interface IAssetBundleUpdate {

        bool CheckUpdate(AssetBundleManifest client, AssetBundleManifest server, out List<string> deletes, out List<string> downloads);

    }

}
