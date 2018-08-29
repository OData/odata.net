ž

qD:\odata.net\test\Common\Microsoft.Test.OData.DependencyInjection\Build.NetFramework\..\ContainerBuilderHelper.cs
	namespace

 	
	Microsoft


 
.

 
Test

 
.

 
OData

 
.

 
DependencyInjection

 2
{ 
public 

static 
class "
ContainerBuilderHelper .
{ 
public 
static 
IServiceProvider &
BuildContainer' 5
(5 6
Action6 <
<< =
IContainerBuilder= N
>N O
actionP V
)V W
{ 	
IContainerBuilder 
builder %
=& '
new( + 
TestContainerBuilder, @
(@ A
)A B
;B C
builder 
. #
AddDefaultODataServices +
(+ ,
), -
;- .
if 
( 
action 
!= 
null 
) 
{ 
action 
( 
builder 
) 
;  
} 
return 
builder 
. 
BuildContainer )
() *
)* +
;+ ,
} 	
} 
} À	
tD:\odata.net\test\Common\Microsoft.Test.OData.DependencyInjection\Build.NetFramework\..\ServiceProviderExtensions.cs
	namespace

 	
	Microsoft


 
.

 
Test

 
.

 
OData

 
.

 
DependencyInjection

 2
{ 
[ 
CLSCompliant 
( 
false 
) 
] 
public 

static 
class %
ServiceProviderExtensions 1
{ 
public 
static 
ServiceScopeWrapper )
CreateServiceScope* <
(< =
this= A
IServiceProviderB R
	containerS \
)\ ]
{ 	
var 

innerScope 
= 
	container &
.& '
GetRequiredService' 9
<9 : 
IServiceScopeFactory: N
>N O
(O P
)P Q
.Q R
CreateScopeR ]
(] ^
)^ _
;_ `
return 
new 
ServiceScopeWrapper *
(* +

innerScope+ 5
)5 6
;6 7
} 	
} 
} é"
oD:\odata.net\test\Common\Microsoft.Test.OData.DependencyInjection\Build.NetFramework\..\TestContainerBuilder.cs
	namespace 	
	Microsoft
 
. 
Test 
. 
OData 
. 
DependencyInjection 2
{ 
public 

class  
TestContainerBuilder %
:& '
IContainerBuilder( 9
{ 
private 
readonly 
IServiceCollection +
services, 4
=5 6
new7 :
ServiceCollection; L
(L M
)M N
;N O
public 
IContainerBuilder  

AddService! +
(+ ,
	Microsoft 
. 
OData 
. 
ServiceLifetime +
lifetime, 4
,4 5
Type 
serviceType 
, 
Type 
implementationType #
)# $
{ 	
Debug 
. 
Assert 
( 
serviceType $
!=% '
null( ,
,, -
$str. C
)C D
;D E
Debug 
. 
Assert 
( 
implementationType +
!=, .
null/ 3
,3 4
$str5 Q
)Q R
;R S
services 
. 
Add 
( 
new 
ServiceDescriptor .
(. /
serviceType 
, 
implementationType /
,/ 0$
TranslateServiceLifetime1 I
(I J
lifetimeJ R
)R S
)S T
)T U
;U V
return 
this 
; 
} 	
public!! 
IContainerBuilder!!  

AddService!!! +
(!!+ ,
	Microsoft"" 
."" 
OData"" 
."" 
ServiceLifetime"" +
lifetime"", 4
,""4 5
Type## 
serviceType## 
,## 
Func$$ 
<$$ 
IServiceProvider$$ !
,$$! "
object$$# )
>$$) *!
implementationFactory$$+ @
)$$@ A
{%% 	
Debug&& 
.&& 
Assert&& 
(&& 
serviceType&& $
!=&&% '
null&&( ,
,&&, -
$str&&. C
)&&C D
;&&D E
Debug'' 
.'' 
Assert'' 
('' !
implementationFactory'' .
!=''/ 1
null''2 6
,''6 7
$str''8 W
)''W X
;''X Y
services)) 
.)) 
Add)) 
()) 
new)) 
ServiceDescriptor)) .
()). /
serviceType** 
,** !
implementationFactory** 2
,**2 3$
TranslateServiceLifetime**4 L
(**L M
lifetime**M U
)**U V
)**V W
)**W X
;**X Y
return,, 
this,, 
;,, 
}-- 	
public// 
IServiceProvider// 
BuildContainer//  .
(//. /
)/// 0
{00 	
return11 
services11 
.11  
BuildServiceProvider11 0
(110 1
)111 2
;112 3
}22 	
private44 
static44 
ServiceLifetime44 &$
TranslateServiceLifetime44' ?
(44? @
	Microsoft55 
.55 
OData55 
.55 
ServiceLifetime55 +
lifetime55, 4
)554 5
{66 	
switch77 
(77 
lifetime77 
)77 
{88 
case99 
	Microsoft99 
.99 
OData99 $
.99$ %
ServiceLifetime99% 4
.994 5
Scoped995 ;
:99; <
return:: 
ServiceLifetime:: *
.::* +
Scoped::+ 1
;::1 2
case;; 
	Microsoft;; 
.;; 
OData;; $
.;;$ %
ServiceLifetime;;% 4
.;;4 5
	Singleton;;5 >
:;;> ?
return<< 
ServiceLifetime<< *
.<<* +
	Singleton<<+ 4
;<<4 5
default== 
:== 
return>> 
ServiceLifetime>> *
.>>* +
	Transient>>+ 4
;>>4 5
}?? 
}@@ 	
}AA 
}BB ÿ

nD:\odata.net\test\Common\Microsoft.Test.OData.DependencyInjection\Build.NetFramework\..\ServiceScopeWrapper.cs
	namespace

 	
	Microsoft


 
.

 
Test

 
.

 
OData

 
.

 
DependencyInjection

 2
{ 
[ 
CLSCompliant 
( 
false 
) 
] 
public 

class 
ServiceScopeWrapper $
{ 
private 
readonly 
IServiceScope &
scope' ,
;, -
public 
ServiceScopeWrapper "
(" #
IServiceScope# 0
scope1 6
)6 7
{ 	
this 
. 
scope 
= 
scope 
; 
} 	
public 
IServiceProvider 
ServiceProvider  /
{ 	
get 
{ 
return 
this 
. 
scope #
.# $
ServiceProvider$ 3
;3 4
}5 6
} 	
public 
void 
Dispose 
( 
) 
{ 	
this 
. 
scope 
. 
Dispose 
( 
)  
;  !
}   	
}!! 
}"" à
3D:\odata.net\src\AssemblyInfo\AssemblyInfoCommon.cs
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 2
)2 3
]3 4
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str X
)X Y
]Y Z
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str	  
)
  ¡
]
¡ ¢
[   
assembly   	
:  	 

AssemblyCulture   
(   
$str   
)   
]   
["" 
assembly"" 	
:""	 
!
AssemblyConfiguration""  
(""  !
$str""! (
)""( )
]"") *
[(( 
assembly(( 	
:((	 

AssemblyProduct(( 
((( 
$str(( 6
)((6 7
]((7 8
[,, 
assembly,, 	
:,,	 

CLSCompliant,, 
(,, 
true,, 
),, 
],, 
[44 
assembly44 	
:44	 


ComVisible44 
(44 
false44 
)44 
]44 
[JJ 
assemblyJJ 	
:JJ	 

SecurityRulesJJ 
(JJ 
SecurityRuleSetJJ (
.JJ( )
Level1JJ) /
,JJ/ 0'
SkipVerificationInFullTrustJJ1 L
=JJM N
trueJJO S
)JJS T
]JJT U
[WW 
assemblyWW 	
:WW	 
-
!NeutralResourcesLanguageAttributeWW
 +
(WW+ ,
$strWW, 3
)WW3 4
]WW4 5ñ
iD:\odata.net\obj\AnyCPU\Debug\Microsoft.Test.OData.DependencyInjection.csproj\Desktop\VersionConstants.cs
internal 
static	 
class 
VersionConstants &
{ 
internal 
const 
string 
ReleaseVersion (
=) *
$str+ 2
;2 3
internal 
const 
string 
AssemblyVersion )
=* +
$str, 9
;9 :
} 