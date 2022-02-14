summary:
better: 1, geomean: 1.008
worse: 1, geomean: 1.012
new (results in the diff that are not in the base): 1
missing (results in the base that are not in the diff): 1
total diff: 4

| Worse                                    | diff/base | Base Self Allocations | Diff Self Allocations | Modality|
| ---------------------------------------- | ---------:| ---------------------:| ---------------------:| --------:|
| Microsoft.OData.ODataResourceBase.ctor() |    1.0125 |              80000.00 |              81000.00 |         |

| Better                   |          base/diff | Base Self Allocations | Diff Self Allocations | Modality|
| ------------------------ | ------------------:| ---------------------:| ---------------------:| --------:|
| microsoft.odata.core.dll | 1.0078836582276485 |             767068.00 |             761068.00 |         |

| New                                                  | diff/base | Base Self Allocations | Diff Self Allocations | Modality|
| ---------------------------------------------------- | --------- | ---------------------:| ---------------------:| -------- |
| Microsoft.OData.ODataValueUtils.ToODataValue(object) | N/A       |                       |              40006.00 | N/A     |

| Missing                                          | diff/base | Base Self Allocations | Diff Self Allocations | Modality|
| ------------------------------------------------ | --------- | ---------------------:| ---------------------:| -------- |
| Microsoft.OData.ODataPrimitiveValue.ctor(object) | N/A       |              80001.00 |                       | N/A     |

