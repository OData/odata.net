summary:
better: 4, geomean: 1.686
worse: 27, geomean: 1.306
new (results in the diff that are not in the base): 2
total diff: 33

| Worse                                            |          diff/base | Base Count | Diff Count | Modality|
| ------------------------------------------------ | ------------------:| ----------:| ----------:| --------:|
| AppDomainTimerSafeHandle                         |                  3 |       1.00 |       3.00 |         |
| WorkStealingQueue                                |                  2 |       3.00 |       6.00 |         |
| ThreadPoolWorkQueueThreadLocals                  |                  2 |       3.00 |       6.00 |         |
| MemberInfoCache<RuntimeConstructorInfo>          |                  2 |       1.00 |       2.00 |         |
| RuntimeConstructorInfo[]                         |                  2 |       1.00 |       2.00 |         |
| List<Microsoft.OData.Client.OperationDescriptor> |                  2 |       1.00 |       2.00 |         |
| Thread                                           | 1.6666666666666667 |       6.00 |      10.00 |         |
| Microsoft.OData.Client.EntityDescriptor          |                1.5 |       2.00 |       3.00 |         |
| Microsoft.OData.Client.ClientEdmStructuredValue  |                1.5 |       2.00 |       3.00 |         |
| RuntimeConstructorInfo                           | 1.3333333333333333 |       3.00 |       4.00 |         |
| StringBuilder                                    |               1.25 |       8.00 |      10.00 |         |
| SByte[]                                          | 1.2075823492852704 |    1609.00 |    1943.00 |         |
| HashSet<String>                                  |                1.2 |       5.00 |       6.00 |         |
| Char[]                                           | 1.0769230769230769 |      26.00 |      28.00 |         |
| Uri                                              | 1.0740740740740742 |      27.00 |      29.00 |         |
| UriInfo                                          | 1.0666666666666667 |      15.00 |      16.00 |         |
| MoreInfo                                         | 1.0666666666666667 |      15.00 |      16.00 |         |
| GCHeapHash                                       | 1.0526315789473684 |      38.00 |      40.00 |         |
| Microsoft.OData.Edm.EdmEntityTypeReference       |               1.05 |      20.00 |      21.00 |         |
| RuntimeTypeCache                                 | 1.0357142857142858 |      28.00 |      29.00 |         |
| Object[]                                         | 1.0188679245283019 |     530.00 |     540.00 |         |
| RuntimeParameterInfo                             | 1.0025806451612904 |     775.00 |     777.00 |         |
| ParameterInfo[]                                  |  1.001926782273603 |     519.00 |     520.00 |         |
| Signature                                        | 1.0019193857965452 |     521.00 |     522.00 |         |
| RuntimeType[]                                    | 1.0016806722689076 |     595.00 |     596.00 |         |
| RuntimeType                                      | 1.0002852253280092 |    3506.00 |    3507.00 |         |
| String                                           | 1.0000849629703055 |   70619.00 |   70625.00 |         |

| Better                                                                           |          base/diff | Base Count | Diff Count | Modality|
| -------------------------------------------------------------------------------- | ------------------:| ----------:| ----------:| --------:|
| Container<Microsoft.OData.ODataAnnotatable, Microsoft.OData.Client.Materializati |                  2 |       2.00 |       1.00 |         |
| Entry<Microsoft.OData.ODataAnnotatable, Microsoft.OData.Client.Materialization.O |                  2 |       2.00 |       1.00 |         |
| Entry<Uri, Microsoft.OData.ODataResource>[]                                      |                  2 |       2.00 |       1.00 |         |
| Int32[]                                                                          | 1.0101010101010102 |     200.00 |     198.00 |         |

| New                                       | diff/base | Base Count | Diff Count | Modality|
| ----------------------------------------- | --------- | ----------:| ----------:| -------- |
| CtorDelegate                              | N/A       |            |       2.00 | N/A     |
| ObjectEqualityComparer<WorkStealingQueue> | N/A       |            |       1.00 | N/A     |

No Missing results for the provided threshold =  and noise filter = .

