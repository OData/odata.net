summary:
better: 1, geomean: 1.087
worse: 2, geomean: 1.014
total diff: 3

| Worse        |          diff/base | Base Median (ns) | Diff Median (ns) | Modality|
| ------------ | ------------------:| ----------------:| ----------------:| --------:|
| System.Int32 | 1.0866205845157042 |        115446.00 |        125446.00 |         |

| Better                            |          base/diff | Base Median (ns) | Diff Median (ns) | Modality|
| --------------------------------- | ------------------:| ----------------:| ----------------:| --------:|
| System.String                     | 1.0238671079429735 |        128696.00 |        125696.00 |         |
| System.Collections.Generic.List<> | 1.0039367088607596 |        237933.00 |        237000.00 |         |

| New                               | diff/base | Base Median (ns) | Diff Median (ns) | Modality|
| --------------------------------- | --------- | ----------------:| ----------------:| -------- |
| Microsoft.OData.ODataProperty     | N/A       |                  |         45000.00 | N/A     |
| System.SZGenericArrayEnumerator<> | N/A       |                  |         50147.00 | N/A     |

| Missing                                     | diff/base | Base Median (ns) | Diff Median (ns) | Modality|
| ------------------------------------------- | --------- | ----------------:| ----------------:| -------- |
| System.Collections.ObjectModel.Collection<> | N/A       |        125001.00 |                  | N/A     |

