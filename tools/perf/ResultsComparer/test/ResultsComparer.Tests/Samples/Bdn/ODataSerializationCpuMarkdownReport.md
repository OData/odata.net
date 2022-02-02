summary:
better: 49, geomean: 1.147
worse: 5, geomean: 1.033
total diff: 54

| Worse                                                                            |          diff/base | Base Median (ns) | Diff Median (ns) | Modality|
| -------------------------------------------------------------------------------- | ------------------:| ----------------:| ----------------:| -------- |
| SerializationBaselineTests.SerializationBenchmarks.WriteJson(dataSize: 5000, isM | 1.0472337412764237 |       8898300.00 |       9318600.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteJson(dataSize: 5000, isM |  1.038105344395138 |       8811100.00 |       9146850.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteJson(dataSize: 10000, is |  1.032133256852358 |      17782200.00 |      18353600.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteJson(dataSize: 10000, is | 1.0298292902066486 |      17808000.00 |      18339200.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteJson(dataSize: 1000, isM |  1.020155200249987 |       1920100.00 |       1958800.00 | several?|

| Better                                                                           |          base/diff | Base Median (ns) | Diff Median (ns) | Modality|
| -------------------------------------------------------------------------------- | ------------------:| ----------------:| ----------------:| --------:|
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  |  1.239975815133238 |      25225200.00 |      20343300.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 1000 | 1.2388368869437911 |      23862600.00 |      19262100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 5000 |  1.233939592367872 |     118341100.00 |      95905100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 1000 | 1.2335430442631166 |      25351900.00 |      20552100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS |  1.231941215543009 |      23740000.00 |      19270400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 1000 | 1.2284255016635326 |     237077400.00 |     192992900.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 1000 |   1.22785264176025 |     252622700.00 |     205743500.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  | 1.2273932985156952 |     123499700.00 |     100619500.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS | 1.2257649648623885 |     118660300.00 |      96805100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  | 1.2254138911373726 |     260084050.00 |     212241800.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS |  1.224718105634736 |     237068500.00 |     193569850.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS |  1.223717766935697 |     126436600.00 |     103321700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  | 1.2231833019136886 |     243478000.00 |     199052750.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2217044955515968 |     260673500.00 |     213368700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2212590203356781 |     123509100.00 |     101132600.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSync(dataSize: 5000 | 1.2198067351065485 |     125474200.00 |     102864000.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2197055673362414 |     129935600.00 |     106530300.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2182187163088392 |      24971900.00 |      20498700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS |  1.218181364804361 |     251591850.00 |     206530700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2181029952311364 |     243142250.00 |     199607300.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  | 1.2158922478078782 |      26332700.00 |      21657100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncFile(dataSize:  | 1.2127900327408094 |     129795700.00 |     107022400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPool(dataS |  1.211470433519456 |      25103000.00 |      20721100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataSyncArrayPoolFile(d | 1.2045993443253906 |      26290200.00 |      21824850.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 1000 | 1.0896632951188772 |      81137800.00 |      74461350.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 1000, is |  1.087516927029268 |      81192600.00 |      74658700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS | 1.0864723653097725 |      81098100.00 |      74643500.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: | 1.0864697517371829 |      79381500.00 |      73063700.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 1000, is |  1.086460402171964 |      79374950.00 |      73058300.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 5000 | 1.0847284060077338 |     406913950.00 |     375129800.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: | 1.0846196980013618 |      81239100.00 |      74901000.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 1000 | 1.0840733306598864 |     813155600.00 |     750092800.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 1000 | 1.0825541506422751 |     792749100.00 |     732295100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 5000 | 1.0816176147981047 |     396905100.00 |     366955100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 10000, i | 1.0812883065928116 |     809794600.00 |     748916450.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS | 1.0804224722998488 |     792409500.00 |     733425600.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: | 1.0802535786075247 |     808588600.00 |     748517400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 10000, i |  1.080199674470222 |     790945750.00 |     732221800.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS | 1.0795865844081785 |     396616900.00 |     367378500.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS | 1.0788779841809257 |     810447100.00 |     751194400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 5000, is |  1.078595167029408 |     394974000.00 |     366193000.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: |  1.078378777559879 |     394269300.00 |     365613000.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS |  1.078107018531213 |     405924650.00 |     376516100.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: | 1.0779754629278921 |     789774950.00 |     732646500.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPool(dataSize: |  1.073098631682335 |     404467400.00 |     376915400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteOData(dataSize: 5000, is | 1.0729443336878959 |     404288000.00 |     376802400.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataFile(dataSize: 1000 | 1.0711602224368848 |      82057300.00 |      76606000.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteODataArrayPoolFile(dataS |  1.069099640229677 |      82417800.00 |      77090850.00 |         |
| SerializationBaselineTests.SerializationBenchmarks.WriteJsonFile(dataSize: 1000, | 1.0179115363164444 |       2372650.00 |       2330900.00 |         |

No New results for the provided threshold = 1% and noise filter = 0.3ns.

No Missing results for the provided threshold = 1% and noise filter = 0.3ns.

