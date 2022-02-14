summary:
better: 2, geomean: 1.000
worse: 1, geomean: 1.000
new (results in the diff that are not in the base): 2
missing (results in the base that are not in the diff): 1
total diff: 6

| Worse        |          diff/base | Base Size (bytes) | Diff Size (bytes) | Modality|
| ------------ | ------------------:| -----------------:| -----------------:| --------:|
| System.Int32 | 1.0003609191021487 |        2770704.00 |        2771704.00 |         |

| Better                            |          base/diff | Base Size (bytes) | Diff Size (bytes) | Modality|
| --------------------------------- | ------------------:| -----------------:| -----------------:| --------:|
| System.String                     | 1.0001873115879565 |        5339698.00 |        5338698.00 |         |
| System.Collections.Generic.List<> | 1.0001313567470604 |        7613856.00 |        7612856.00 |         |

| New                               | diff/base | Base Size (bytes) | Diff Size (bytes) | Modality|
| --------------------------------- | --------- | -----------------:| -----------------:| -------- |
| Microsoft.OData.ODataProperty     | N/A       |                   |        2880000.00 | N/A     |
| System.SZGenericArrayEnumerator<> | N/A       |                   |        1604704.00 | N/A     |

| Missing                                     | diff/base | Base Size (bytes) | Diff Size (bytes) | Modality|
| ------------------------------------------- | --------- | -----------------:| -----------------:| -------- |
| System.Collections.ObjectModel.Collection<> | N/A       |        3000024.00 |                   | N/A     |

