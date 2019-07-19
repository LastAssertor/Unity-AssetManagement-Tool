using System.Collections;
using System.Collections.Generic;

namespace LGamekit.LegacyAssetManagementSystem {

    public interface IReferCounter {

        int Count { get; }

        int Retain();
        int Release(bool force = false);
    }

}