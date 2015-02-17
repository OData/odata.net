SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/****** Object:  Table [#Songs3]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Songs3](
	[Id] [int] NOT NULL,
	[SongName] [nvarchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Songs3__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Songs3] ([Id], [SongName]) VALUES (1, N'songname1')
INSERT [#Songs3] ([Id], [SongName]) VALUES (2, N'song2')
INSERT [#Songs3] ([Id], [SongName]) VALUES (3, N'song3')
INSERT [#Songs3] ([Id], [SongName]) VALUES (4, N'song 4')
/****** Object:  Table [#Songs2]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Songs2](
	[Id] [int] NOT NULL,
	[SongName] [nvarchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Songs1__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Songs2] ([Id], [SongName]) VALUES (1, N'songname1')
INSERT [#Songs2] ([Id], [SongName]) VALUES (2, N'song2')
INSERT [#Songs2] ([Id], [SongName]) VALUES (3, N'song3')
INSERT [#Songs2] ([Id], [SongName]) VALUES (4, N'song 4')
/****** Object:  Table [#Songs]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Songs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SongName] [nvarchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Songs__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Songs] ON
INSERT [#Songs] ([Id], [SongName]) VALUES (1, N'songname1')
INSERT [#Songs] ([Id], [SongName]) VALUES (2, N'song2')
INSERT [#Songs] ([Id], [SongName]) VALUES (3, N'song3')
INSERT [#Songs] ([Id], [SongName]) VALUES (4, N'song 4')
SET IDENTITY_INSERT [#Songs] OFF
/****** Object:  Table [#Runs]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Runs](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[Purpose] [int] NULL,
	[Started] [datetime] NULL,
	[Completed] [datetime] NULL,
	[StartedBy] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[RequestStarted] [datetime] NULL,
	[RequestCompleted] [datetime] NULL,
	[RequestStartedBy] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__Runs__0CBAE877__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (1, N'öÜ', 2147483647, CAST(0x0027D85F0150ABE4 AS DateTime), NULL, N'ZvvÜ©ã|å>~©+U<z+OßvÄýhoåßýÃZÄðäh|Az©ä¢rÜ_hUz:¢ÄZ|U~ãrC+hC+öÃoZO~Ð@ßrO¢>|ª>Oãîä.rv,Ð<*aö:£o|<©>ªUö©|ðß>CðÐAUÄð@UvüÖ.Ö|ÄUÐ~*|AbaåBåðîUZ<,ÖZßÐ ÜU@hîz@îBãÃU/ã*:öbäÐhhßa¢ýý>+£<Öüîö*ã/CÜ', CAST(0xFFFFD73C001281E4 AS DateTime), CAST(0x000BC9D9016E94EC AS DateTime), NULL)
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (2, N'ðßröaO¢.@@îza_/+öv@åa¢>übUohßÜöO_+/ãZßüÐ ð ðr:</~©Z.z_CzzuäÖA>¢Ð', -1577163515, CAST(0xFFFF2E4600000000 AS DateTime), CAST(0x000327A30114D074 AS DateTime), N'Ü|bz~ÜªÜÐ|h<_åaUh@böuÄãz~*:Ãß*ý~Oh@üAaÄ:výZö<vAäÜ,übAObbÐÜÄuzðö,|_z,*AuCß~Ãu.åßz ªî.+ð_ab/UvZ<o bî+ðü¢>ÖOãv/<ã©ãö,îÃa@a£ýz ðÜöãz/*O>@å*ÃBoU_£C@r', CAST(0x0009C6C601711E24 AS DateTime), CAST(0x001552B700EC9BE0 AS DateTime), N'îÐð£Z~üUaöah©O bbu~äZ:ýåßCo,¢öÐüA£~ÜüÄ,>Oý_ÄC+_~bvã+å:u/Öbäo/~¢bubCA~_B ßB©ZðZÜ.~|z*B+b|/oAîãUbª+<<bv>a@O>_Üßð@<v:|åvZüäü|ª/@O>~Abð:ßÄob_Üo~ßzÖ>¢|¢,©O B>¢BÄî~aaaüÖÐab©|Z> <C,A©ßBßü_CðãÐãöo¢:OüÜo~ã_a>|åbBOããüCª/îßßb:åbðaOC@ðãz+ÄüßOCÜªUB<AÐaåäã|r_+OÐÄÃß¢a /*hUä:haz.zh')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (3, N'üýäzîhzZöÐzörývC,>ÜZßÐ Üå_Ðä@oå,o¢ÜÄß<Ääah.,öîUoÐraß+ðÐu£B.+,/Üßäîß~r_zðßîv© ðßýZ~C hbAUå_bÄb*<~+ª_AC©örîBa:£bîÐv/bßz/£Üä_Ã©<~å/.ÃZÄ+Üå¢£ð/bb|ªß¢*/¢v©vðöÜO|üZÐzr|Aüßr*<_:o,aªÄZöîAhhUOÖ.*Öªª £+rCã>_¢a/ýübß@åð*båªbU~Oü,Z_//ÐåußßOîÖuÖzîªhOo,u_/,_ÄÄ|CªöåBUB|ývbäýÐÃAªãhzO£:aßabýabOo>U', 2147483647, CAST(0x0000901A0000012C AS DateTime), CAST(0x0007260F00DCAB68 AS DateTime), N' ªåBz,>ã.ßÃ~.,<oöBBOB_öbð+ÐbCbÐÐªbãA~î<ª|oªU>|BãO.aÜa CÜ©ßoö©.Ä*büý _:|Aª>ªööÃ~+vååÃböÄvüb+ÜÖ_ðo/UO@~ÖzÐäüo@Ðo', CAST(0x002D247F018B81FF AS DateTime), NULL, N'©Ü@AZCUÄ*<bÜZb|:@Ã@bß>~oãbßr_ßu,|üåh¢.übZä.ß£bÃBöZ£h:oýor+Or Ao|UÃu r.ãCÐ:ÜðBÜ@Ü,ÃUvoBö,üÄöUuÜÐUÄã|üã+î_/_,ý,bb/ä.ß£ßî,oÐã+<:~åöÄ@uvu_ðaÜÄß¢Ãöýå£@,ßüB/ß/.uaa/bã£:våäßrz>ßäBbÜ:ßã<ãörbý¢b.ßß îa*¢üüU©CAãzb ou*Ä©ö/O@~h£~:£ß/ý,ý~ß@<CßrCÄ:ý¢CÐã,ÐÃoäåÐ<Ö< ,ÜA ~ãîBÐ*ÄåªãÖ :bª>@ã¢UbÐã_Bã ÖýÜ/.b,åhÄAý<ÃãBzC|Z,oüß_oßvãa+ýîAöîß. Ubr:ß>ão_u£_/_|ðªaýåvZhbh*bB/Ðövåß UrÄOzÃýUAO<ö/ B*îã_böðoz::ÖÄßv+££u ÐåßÃrvbBä©.ªÜ ,b')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (4, N'Ð+ b.ÐäöZ/Ößßzua+<bh*ÜîßZ£Aãý+.uözîäa©<bãUý¢ bãB,ÖüðüZaö<>ÖÖÃaüîa_ä@,+aÄýýzBßv>Ußö:u.>U£zö+>îÄ,ßZBªßO ÄaÖUðýýîora@üÖ|Ð<@Zuu/ý>ßvª>ßÃ©Äb<u/:O|v©,ßO>,Ö¢v_¢b>öCvAv/£,<B:@z/>@hZZöA_ß', 1754781997, CAST(0xFFFF2E4600000000 AS DateTime), CAST(0x002D247F018B81FF AS DateTime), N'vb~rC*ü,rÐðåOZ £OüUChv h<,Z¢rUAh.Uã,ðBzh|Ðb|Ah+@zoß:väBaÐ:ÐÄb*röÄÜ:ð:öÄb£C|,rÜB AÖzb/~ä|hß©<+~bu_CäU©/b+:ÐßozÄßü*,ÃzAo@h', CAST(0x000A51260107F37C AS DateTime), CAST(0x002D247F018B81FF AS DateTime), NULL)
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (5, N'bb äbðä©~Äö~ÄÖÄv/aÃCýîÜO,ªÐ Ü©.BuAÖCüãÐb>ÐBªb|ß+u¢ ¢~CÄhvo<:O_åðZä+©aªzªZ£¢vßoCßß*Bh ß@B ªãß*väüãOzuOh*ª Ä ð</*BA©©ã/Ö~< Z*,Ö>vaß©/¢äå££ý<ßª:~ªåvÐÜÃ~A@B~ãåîßüäã: hßaÄUhaAãööýaÖ©:hAÄ~.Üh*AaaÖäA¢üoh:ßå_<¢©|O~hªßhU ãßhåî+©©ÃuhvoßzýßåZä~~_Bîã~|r/rä>_rßãvubhuäU<..,voÜöã,ÐUvAübßC:vAZÖ¢~>h|£uÄ+Äzª>/OZA£vaCßö_ßzoZAÖªur£ÜßÜauZ¢©+_ü/<¢ªß.ªÖv£ã£ö*U>ðÃªCb.ä£,>a.ý,:/.~*ã', -1446905558, CAST(0x0000901A0000012C AS DateTime), CAST(0x0000901A0000012C AS DateTime), N'aöoß©üßÃüÖ/åaªüuz,OÜåO.u+ý.üv<vCª>äÄUÃ*:OÃ<©ýÃ u|:îößÃAß_ð:©|ã/><zv@Övý¢bÐªrÃÐU©£r,ß~ Zð:AOzu©å*Uýr©.ß:z:våBBãhß©ÜA+/¢îhA*bÐb©.+ãUöÃ_~vãÐB*z:rã¢î+£/£@ðªBb£bÖÐußä@Uãå/vB@O¢ ZªÃß>/ãÄaüÐ z ª>ðOBaå_/ðüaÖ<B_î:åª <*ß~zî>Üîßð@_ßÜo B¢/ÐOÜä>Ã+ª:bv/ÄahhBªÜC+Ü@hb<r£ ,|*vo~z~üb¢_îzã@<ß_Ã¢bÐ @bü£¢ßa<Ävb<îo|/ã|h@h|<:©¢|©©öÄ ãAÖbª£üoboß¢u+ªzUCäbðrröª>üav*ßÖ|Ö£å.>ßOªü:ýÖãUzaU/|B£,¢ãß<C©öîÄ*bb<ÐÐîÖZ©/üÖrOÖv©ýaðu©î*abªAÄ@£bÖ¢ÃÜÖ~v*©v/u/å¢¢Äãýöåßh,oCÄÃZÐ~hßåa~>ý@BÐ©UöuÃðh.ýzä@ÃOö>Zª£ä|', CAST(0x000651D600DEDA64 AS DateTime), CAST(0x001245F1004E4260 AS DateTime), N'CÄ îur_OO<O~vZ.ÄªB')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (6, N'Br,äUo¢Zvðv+U+äª_¢<ü. C', -982374538, CAST(0x002194C2004469AC AS DateTime), CAST(0x00120EB90083CEF8 AS DateTime), N'öð :£b~vîUuß î.ßOüªÐ£Übv.üäãÐzUð_./öZBü.+h.åî£ð,+ÐßÄÐBroäbbÃOÖ@~ýzvð:<@bar*>*r*ÖA:<a@_O/uZBrAZu¢¢ãå~+aöîuÄÄ_ÖÃß©ÄZ©Äv@b/CãÄo,zAÄüÐå@zÖ,A@~Üßã£Ã@<.>Ö¢>*BÖ:<_zÜ>î©ý|AßZ+üî¢rÃÐv~:oOÃüßZB:+ÜÜãZäÐÐbåCäB©ÄOb@äýZ©+<äa,Cb_b,åB£b@@©bî<ªãhÐ+<<>îZ.CoÃ,Zýv:ª£@ßÐüýa£©_äßbßb@ C¢î£|£ÜüÜðö£U@,~zßBA.Ö,<ßã*/ªA £äöä£|übåbÖrbba/A:uÜaü>ð+U.ßÐZ *Ö~zßZ£Öa Ðrü©| /ß. ý,U ©*A>.hh*ªÄ,::Ðuh,Äu/vÜ_Ußîýý ö ü>v¢/vh*är,ªvvBäÃªvöZoßbîßÜÃ>oha£+/örCÜåU**UäoZý', CAST(0x0000444F00AE8814 AS DateTime), CAST(0x0000901A0000012C AS DateTime), N'bßÃrÄ')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (7, N'/ãÄb,öåßh£|ß+Ãðß_OÐ_Üý:ãÖö¢ ¢Ua,O.ß@¢r<u,o|,Ð|ÄzhýhaýbÃ:+åz/z|rî£O£öî.O*Ã@h¢.bBOu r£Cý.CÄ hÖîbîãü©Ð|ðÖ.Äßßz:ðbAzÜCrý*_Ü£Ü_u|v.:£¢z£Ö*_v<ð<ãzÃör£ZãýÜãî<.åbÜ,@ã:+*£ãª.ZüUäÐbåÄ<üã*~v//©ßÄßUåvÖåöbA:üAC~uaäÖðBãªÄ,CöAð|aöÃßCßO.Ã<ýßýäU@ðÄÖ+v|ÃÐÐåu@<oßã', 1106909481, NULL, CAST(0x0000901A0000012C AS DateTime), N'ÜAª', CAST(0x00143EFA002C478C AS DateTime), CAST(0xFFFF2E4600000000 AS DateTime), N':åOuZ|z/UöBU~£ö_£hÜ:äÐUÄÄý*£,.ß/a<b~ü~|:r|v*CÜr+©¢¢ß@,åÐ£<@zu£bãh©uC~*~b~ vZ:îªðÜüýÜßaî¢~ ö>bvU_@.r/_BÜ A_urh~ZÖä~@U©vbhÖ~@v,Ba BÄÜAÃo,ÃO©ÐUßÄ>ýbr¢<Ðýåv+bÃü~@ðö_åB.ßUbÄrãba@ß AB@ur:_~Bã@£ª<.C©ÖßÃîªBOßhðvîðäÐ~Ð£äbzÄ<azý|z~/UÜÄ.vÐB//a*.b_v©OüÜbåß|b*AzÃªª@äuÃ©<O@ªý<zuäÄî£öÐ¢ß~@@|.u©bu/,ãß<<O+äÜ<©+ß~|ªÃ :,*öüßð ÃCvv@:,|b:.+UÄZböhhuÐðaß@ ªãb:åa|h<Uª@ürýu>BO BÐCZaß£ÄªÜC/ÐOä_<¢~,ßBUA')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (8, N'OöU@/î¢OuäÐß/b*ã_ÄCoßß*/Ðä@Ü@a>bß~£Ä:B*uß_öýßÃr_bü_@îz¢+ªßÜ|+ä©u+£üä.£+:+î*Uã*ªb©ýÐ_<üO>övC£zÜãüåÐªOÐ C,~bhaü,_UuÄå,ChÖ,:<ðh_Ðh~b,+CÐ. *ä ö,ßO__Ðä/ªî/ãÖürÄU.üääö|ö~A.>ß,__ÜO+~Zö+.©Z_Ãª_ªo@Ãbðå:b:vuü@z*¢r+îb|ª¢/zb|r~,ä', -64120314, NULL, NULL, NULL, CAST(0x001FDF40014DA548 AS DateTime), NULL, N'zbýäBz£*zÄ<b_åvÖå v>b,îðBb,@åÖUÐ<Ã + r:îðb|¢_+Äbã|Ð b~ãîobÖAðh:|:Ö|ã|å/Ãä+~aãv,Ð*z©zAîÃåã.rv/.üß~ýOßßä@ühb:Ãaãbî©AÃª:o__bÃahÃoU:,Ü/ãB~ð<ÜÄOü:ýäa*zª,Ä+ /:¢ÐCä+vüßazu,ußbCußÐü|BbßýÜ+_äÐA:a,:äß¢,/rO:Ã¢ªh<_Z>_ª:î_£Zäªî/övvÜ,ß+rz~.zÖ+£,O+¢åa|U>ã_<îu@Öß_ÜUO.äu C ýîCzÄ_îvC<üa.+AÜðã*:ýÜã¢Öüh:ü ozÜ©~ +_ãöaö,vß_+äo£ð,ü©*rb<_Ð,:zß ©UÐßÄ,,ý,/A,~ßuªZZ .Ö B +vîaC_|£ªðoz¢b:U_,|u ß<Ã hCoý+| h.//>+åäå+Ü©¢ðüUãbå+Öäý|ÜrÃü/,©u*ãor¢aÐo@Ä*üAßb,oå î>r|v/ä©ZÃî.<Ã_zßÖÜ¢ß Äãb@A_ühZß|bÜÐov')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (9, N'Ö*:<ªZäv<@v¢,U>|Ü*@Ð. Ão,Bä.ðßo@äaä<åUUÖ©Öb:å£/z|rößÖo+.£Zb>//*©*ÐÜ+,A©_CÖb£vUvÖ©_ãÄå+ÜÄ£/ß~OÐzu.,Boåbð__,©ýBÄo,Azãîª£ÜýübÐörB+,:ãuåoCåª¢zO<ÃÜªðZ.|äå.¢ÜªBOßzü :AZ_U<vUð/uvz:zzO:ß£Ü@>>Au:vbðbz£üÐ/.U+/aO£.£hzzzßa*>z|ð_Ü>aAaOý+ð¢*Ðäåðb+£ªåBäîîoüvîÄß .ßî£Ö*.aý£AÐ@ü~/îBZC©öA/ðªBåAðB©hZððüUOÄü¢¢Ubh+ö|@+ß.©£:Ü:Ähv_üb|*~uA£<£î|obzß,ßuoz@_vv:¢:o>@A:/£|ÃÃ_oîðbÐîä@ªoýbððBU:@ZBh~~>üCåBÄ,ÐBÃUbv+:ß+,ðAzo/', 256524076, CAST(0xFFFF2E4600000000 AS DateTime), CAST(0x00012383002CDA08 AS DateTime), N'~ /BªAÜßu.å+©äãäaÃðÄu|v @@ÖÐ@OÃÃCä<öö*r ¢CßîZbðv_ O+<åOAbB|BZbvAÐOUß+îð£<~BCÖ+A:ª>rrýzrUÃ:åãÜCb|ãÖoBßBå¢._îîÜbvÃbC>ßäbA.üahîabä*_@varö¢>ª|ß:.z:¢rrß©¢vÄÜÄÐhßüü¢ ©_*vßU<C>ýOzv~a©|@ÄßZrÐvß>Z<ßß_¢ð:ÜåörßöÄ©', NULL, CAST(0x000198760185BC44 AS DateTime), N'Ð<v/vßb:ßB:ZüazÃ:|+ßßhÄ+ÃÜBªABr,aoÐ |B*h,zbäÐ@ýÄ*<O>bÃ ªÖîÐ>Ä:/ÄåbZ')
INSERT [#Runs] ([Id], [Name], [Purpose], [Started], [Completed], [StartedBy], [RequestStarted], [RequestCompleted], [RequestStartedBy]) VALUES (10, N'ýh*rU.<bã¢OÜ,v|ðÄ zßh~©*ä.öå+ý£uO/+*ãZßZã_ðv|Ä OäÜOÖýüh,<ý*ß:~ÃäüöªöårB.>Aýª£,ßoBÖa~ßîUrzüåãöaßUãbZ¢C+z£ÃÖUöý@ÖbrrÐÃ~BbbÃ,£ð U<>rÐÖ~BOAÄ:*', 102799233, CAST(0x000C8C75016FF5A8 AS DateTime), NULL, N'ÐZC©ÖAuðÄÐ+ :ZÐÖvÜOüAoaUuuý,,OäÄ@ªÐuBba_åU ýbA/Öð_öýbã¢~vå:_Aß@@ä©ßã_>©uÜ¢BÜßaoOÜu:äUä©©ýhür~<@bÄßC,u~u/åÐu.ÖãÖUÄZÖß*©Zaahä,_:¢zBAbAîÖüOaAü@£Uðh,Ä<zÄÄÄb>C+bÄßî/Bî ÐÜÐ*öz~ß ÄÜ*ÜrÐaãü ª, a@ß+å:rh_.Ððr+£*@ýCð+*ÐÖªåýßb:Üzð.Ör||/C~rO+¢äa+*</ <:ö©ßAÜa:,bb¢.ýßª:ßö|a Oo+hÐå¢:~A:ã<:B._ß.£rCBÐrårÖîÃ<Ö:CÖ>/îbhãö>bO|> @Avoo*hbb¢ðã©ä_å_Ã+*Ä~ü', CAST(0x0010F4DA015DA628 AS DateTime), CAST(0xFFFF2E4600000000 AS DateTime), N'b_¢+åUä/ ªCüÃ/ßür:ü,ª><Oh¢ðb¢.ä@î/aBZ:ßª©/uBv,ahO©|ãããa_b/UU ßßîÐªbv/Ä©Ð©+ÐOZ+ðÖ¢ßåöäA©b/<Aãu|Zå¢z*©zÐÄ:ýßO<CãUÃ@<¢@ðäbÄðAãüÃã>Cbã|üav:åb.üAärð~u* ÖðÖüÖv£ÐZîÖÜo b.vZZ©UAÐ/>îaäÐBUÖ*>ÖOzåßÄ+@Oü £ßÄÜß£zð©BÄrz:ãUßaO©ý/©~îB+<|ZÄäÖBäCzbð:ðß©.Aüa/ö:~@ýz rhCåÃvã+,+ä/rªÐöÄbÄÐOÜU|ðBö@CÄrä/Ü£üOOuOB/ZAo_ßßu@ä/Z~o/å|üª zU©Oaý/üÖÖ_ã')
/****** Object:  Table [#OwnerDetails]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#OwnerDetails](
	[DetailId] [int] NOT NULL,
	[HomeAddress1] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[HomeAddress2] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[Level] [int] NULL,
 CONSTRAINT [PK__OwnerDetails__0519C6AF__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[DetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#OwnerDetails] ([DetailId], [HomeAddress1], [HomeAddress2], [Level]) VALUES (1, N'ßBä*ÐüÖÄã:|ö<.,übAªrÃbB_¢Ð~ÃzZAßBbßßößÜßßßÐa Or©O|BBå*ýÐa Ða,b._/Öä,ª~AvUß+î*bu/_î@ÐrððÖbbßªbÜ/Ü+O|| ßb~@ßãbüb+ðCz:<@© @ßB @ß.r:+ª_o>£BÖãðu+bäÜýÄä>Ö öUÃäbÖu:|ßC¢,Ä+_r.<rÄ£~:ö/ü  ßðð/h.*äz:åäZvhCÖÃh,:>:A|ªz£uî¢ßîCÖäOrvZaÃîü~ /+:*rÖßðåz<åå@BÄ¢,ãBüo@vð_.ã,Öª</uOöräÄåüüU/,z Örzb>h>*vãO/£ ü_CÄý:£ h@ÖÃüCaðBåªu©£BÄB*öäîCa/b_ý/CÄ,ä©.bÜZßO+**Özß.£ /.¢£>ß.A@ ¢.h|ß', N'Ö|vªz.zO~>|+Ãª£î©ýÃBÐ*äýavüoÖUß_+ßbã,ÄCaz<ðZä©ßÐý>:|BZÐ,Äv~A/h. Äaýå_ß>öåÃÜv.|h*:UovÜu/Bb©hhhB|Ob~~ã©örrý@Ã.><ühCU~rüo|<|îßä©+ßÜCªuãüz©@büvv@A~ªÃýaî>*rîABvÖh _¢ÄîZåUhoß|boýîzü_ OO©*îZÖ, ßåAð+Ð/¢_¢ÖÃ*_ßr*/rrýÄhÐ/öhävß¢@>ð ZÜ¢Ä/|vÖ*ß¢ã£ãh|ä|r BÐZ>.~|+Ö', 0)
INSERT [#OwnerDetails] ([DetailId], [HomeAddress1], [HomeAddress2], [Level]) VALUES (2, N'_/ÄÐÜ£ÐößößC>ACuÜüZªu_<hZãýîa*|.,.zÄähß_ªhö*ÃZB_U@uvîãýýö~/ð£ãö:++ÄbÄ.+¢oÖýBªh/ýÖ/£>Cz@|/zÖZÃååÖr>ü |Aããh~Boãî', N'h/OuO/ bäo¢,£BöZäuåBoC/Äªß*@u*Ð_ðöhvÃ©~Üoî:.,/Ob£ßzªaZ£Aa:ª*bßã>.b:ÄöÃªO£CðU£azo£zå.bh@£ZCUUbuÃ*büýzü<BürCöî*,+ýh/ÄÖð/:Z|ß.ã/*C./|rÃaarOuîrðOªZåÐvB¢@ð/¢>ÄÖhbv_ãa_<ÜrÃühU__Au@Cö_@BOv<î>h|OzUbCö++ð£h>ÃbZü,ü+..ü~ßª@O+~@¢ü££>©år/~£äª<ßAãAåZBðhÐorÐ//~ÜaÜ,aoäb|¢bO_ÜÖ_+Cý|©h<Z/Ããî<Zð:Üãß£Bu:£Oß<vuî,*ýªãª/äð£@<ö zü|vÃ¢.ÄüB:o©*,å£rb@z<ðä/ aîðOCªvh rUu~ª>©,åhÃöB<Aý ,/AßAÃß*¢@ßÜýäî_aO£/ª<A¢BbÐÖ+î.årßrßC>O_,uüýåªåZß.BZ,_Äð.,Ðý', -2147483648)
INSERT [#OwnerDetails] ([DetailId], [HomeAddress1], [HomeAddress2], [Level]) VALUES (3, N'r£b/>aBb,>AÐ<o/üüßh<£bZ.ÄÜO| UvîüaÃ£Äý~Zz< .+©Uã_baîü/öC+£ÄÐã@ÄhUhÖ<:ÜUýuãOoÖäbA@ AUU+ð_b|Zbvä/ÜÖOOuîÜ+ÖãA©CUÜ*>~Ãr>><îåru,î>OÄ+,,,|©ßCÖüv.rÃßZzrÜ<~£öZZZzaÃ,<v£zZÄä+ßãÜ|/_ubÃO,ðbÃÄÃhÄ:CCZ+ß.+<ýÖî,C/ö.öÄau,bu~C¢Üzß|üO~UÖU>:ühbbuzßÜZ>hvÖüh>bbåÃUO£bÃ/ßÄßäÃÃð<Öäo ãðß**ZO@ßýAbOÃ+ßuÜ<a+ß>bö.ß/*vuv*Ðrüîîã+åo', N'ÃÖÃý Cvü+höb.Ä,ð<ÐO_ÖÜÐ/>,î öChuoÃ,rÄßCuaUårßo_O>îåhüBbîav>ðuöAvaÐåÄ Or©¢ü_@î_£ äÄßAuýUb£>r: OabrªObvßä Ãu>a__ý<ªob>haAa<ßbßýÖ*/ö<zZ*~bAã<*ý<U@C>ÄäoC@ðr~O|:ÖA öåbbbuÃhaBb£££A_UZüzvåO /|BÄOAªZßo.C.O:åß~ãbbäv__¢zoÜÐbbÄÖ£>Cüöý<Üä>UðCbãB>ÃbBU|Z_O_Ä:>bB*.£_öÖã¢äªzaÜzª@U Ãu+aª>bu|ß.vAvÜ,î@:_å ©br>ö|@U|a<CßäÐ*ßo:Ðrbr~~AU+.BbÃ:+_zßB|@Ã u,ßî:î£_ÄzBhC¢ãªðî hã:üÜ©/z/_Ä~öAåßßß©~ Ðä,ÐªzUð:rO£r|<Ö/aua~uýýÄhCUßAßU ý.übuO+r,ý*_ö', -1536317320)
INSERT [#OwnerDetails] ([DetailId], [HomeAddress1], [HomeAddress2], [Level]) VALUES (4, N'|Ð>ä.A:OýC.+ðzro@ÃrãäßOU£/Ü,£äÖ£AßÜvîB£rÄOrZ|üª¢_zå+Oö~Ð_UuüUb_h*Ö h_öU.AahÃ¢îÃ©ßhã*ý,<BubÄ©åÃö¢£bðß¢<Bªb: OßîC£|_hCuOhüZUî>u~@*b>A|üåA bUZ@bzz öbB _ã,:ðü@,|ª+:ðýCZä_ÃÃ>ßbýå¢ðüð~|o>OßuC~£a+£Ü/äã~bra<,,.ãÐÖ,|ZA_:ýbrüÐoåOvî© äA¢hb', N'ÜðCoÜb*B<Äa*üßOÜ,©ääîzãr>ÄBüÃhv@vOÜ_äå.å,b.:U_Ä>ÃvU<OOö:Bð©bý,. :>BÜb¢ð>Uý<ãî+ööý~v.åß BUz:öu*åý£baÖ:ö:b ÐurÄC|ÄýÄB.Cý:£Oa@ÐOýZîB<Oã+r>AhrBo|h,hÖÐü@v<å©¢*|:.Äå*: BuðÐ*ÜrbZîîZbbª,ý:aÜ£¢ª>ra¢b¢. ÐÄUAorA+OzrrüÜ Z@ãAÐ. ©Ã<¢/@*oh.ä|o~ãa:Oöbã:/.¢ÐåÄBuªÃð,A_|_Ü@Aaoß ,ªÖ/@+ãÜ*>uvbýÃ¢¢bðßO£Ü_Z/ð./AãZ£BuChv~ãÖî~îßv@_bå.öUüoÖãrob:/*z£ð¢üÖ~>*>BbZbäÐað+ |¢îb©ð<ZðC>, |vUî£_Ü>+b:oîäÃü:hhbBß©ÖäöAZz|uýÃÐÐß¢ã++ªãO|/îÜbÜz<ý+zUÖª<bÄ@<rßã::Ohî*¢ÖÖ<v¢Czö', 124883620)
/****** Object:  Table [#OwnerContactInfos]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#OwnerContactInfos](
	[ContactInfoId] [int] NOT NULL,
	[Email] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[WorkPhone] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[StreetAddress] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[City] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[State] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[MainZip] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[ExtendedZip] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__OwnerContactInfo__0AD2A005__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ContactInfoId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#OwnerContactInfos] ([ContactInfoId], [Email], [WorkPhone], [StreetAddress], [City], [State], [MainZip], [ExtendedZip]) VALUES (2, N'|UÖ*ðo¢aä£bh @++h>BU>+U aÃ//bªO, hö>.A£/ßÜ_rößåÄ/Ö¢bÄzorý/*UÄOöU<©ßO~äöBä©/Ðßb+rßÄazüC©aýÖ@ý©.Ä£ßZÄZýÄh|ß_Obå.+ðbo/ao£Z*ýßrß ü.:öÜî_ Brb~:@/ðªh|,ªö@üÄ<Oa+Zr@zUãCÐCB@ß~üü*.Ähã.,©¢/Äaî<uzÐ_üßa|ZrÃzr>Ab|Aü|ªzÜboöî_åä@ßuvåßÜZªOößh>/@uå¢~åÃA åð@Ã+aå<Ã.©äBðªuvv.ßªßOvª¢ÐaîZüãÜå,üvBðªZ<BÄß~/ZZ, ¢vuÐbÖvbÐvå|./ãBr_£öO*ß@CÐ:/AO:Ð>ü*|_o raaO .ðÃvý<UÐ¢/', N':Äª Zãb_ßhb/bZ_Au/öoo>CrAO:_öÐBbßbBã|aðOü£>väUOÃðu@O>ýCü~.ZB+>ªBba¢aOÄ<r@ÜO@ýä*ÜýÖªÐrî|ßöhUoÄßß:A.uãªO>rÐz©ÐßªýÐb@Cªîååh:ßbb< >ÄüÐ<|¢>oßv,ýîUã', N'o>BrÄO|o_B,ßh/Ð.ßöÖBh.B_*z*>îüåªßÃîýuZaÜo A_B+£ß©|BZbîuÐ,_ä,ãbU|rÜü:_Ðr@öýðß+|/ü£_ã*åª_î<oª©/ªöCb<* ,rvð*~ bý.+ãBåÖZuî©ªubü,h@b*AßÐîrC|/vîýZv:ðbßÖ~_ã£îoßÐB*uåÜß>ýbOÃ:¢üzzAzð+*Ühî', N'ö@>,î|C:Aur+Coßv.Ð+ hªU_ª~U., r Z.åý<£vC oöBB©OÖ~b*,_vU*ZüÜ zÄbÄaåbÄZßßÜ<Uýü£h/ö,>ß üßßAo_Ã@åöß¢Bubu~<<ßZ~îZvvÃh_ßößåÐÐOBA_ÖBZå_/+å/br+ðaAb,î@Üåã|übaýÐ*>åC_ðÐÃaÃ+ãî,vb :aßäÃ Z~U©î_zß@:¢@ðOBßýZ AÄ', N'å>~<£ªýOå£oª@zvßoÃää+vå åu£A@_£¢Öî*Ozb©u_vÖäÜ~|@Ð:O*ã©z¢ªv~.üö© ý@ã¢öB>BA£zC@a_z©hzb+zßbªÐ|b:Uv~~.äUÐö|£öbzr+OBªÄZÐß£¢üÃÐÜar@:£Ü,Ä@Chz~Ð|£BZaªÄ.£ö*O@/öü/O>üð+C>._ß£©ý:.Äavý@©Ãhb~_~B<vöÜ_©üßªßO_ªå:ßbÖ©<ö:>oö:Ö>+î ã_,öväåCz>ªBrB:îßu*aãåBoaÖÜßbð:/Ä:*arðrðo¢+ý +zb>_üZaî>ö/Uð@bbb+AO+ÐÐ/Ääa.ãr>Bã¢:Ã _üO.ßü,|üa©äB~ýöha|*bãBvªbZ: ,zb+Ä@.äÜ|Ü åÐä*|Ð¢@oÃ¢', NULL, N'ÃäîßÖ©öu|Äª<,b|B+/ßaOBuðAz£öOÄ©oA/ß_öüßB,£OoÐB*|öbzrB*Cª¢ýÜ/AaAäA/:öUAh.¢Äª@ðbröüBýC,<bÃ uîür*b,ª CZ£. ððî|,Ð~Ã|Üß<ðað:|+Ö¢ACouv¢uî~öðz|ÜbO>î©üÐ_*O')
INSERT [#OwnerContactInfos] ([ContactInfoId], [Email], [WorkPhone], [StreetAddress], [City], [State], [MainZip], [ExtendedZip]) VALUES (3, N'h+Ãå|îåÜzßÜ©ZÖ>©Bªä>ðoðÐö£Ãßî<Öýã,>ãuÄ+a/Ü<v.ß¢î:b/Ð äöü:öððrbåßbßãîv£:+uãboÃ© hAß*äAA£ü:_,vß:ß*î£r¢ýÄ£ß~b><ßßvÜ¢ßå.ÐAÜAhä/ÐvO~ÜAÄvA:Ãhzh_vü@OO©vª£ý~,¢ååO>hZð_.<Ð:B@u ð£~ZOÖß>ö©Ü:£_~O¢b+ovraAraä<ßÐBhAZÄ ýC:CbÃ_:>:îäå_ßahbbÐ£_bOî Ãu~£/*ãvO>¢vB~©|@ÖCýªvîbýüuüÃ:bZuÖýzääA_A¢*z,åý<bbA bîÐ/z@£<Cä|Ð: b ~*Ä£Ð~üüî~©BýÖîª.ÐuzA_ý*u_.Obªäbã£åÄ++~<öÖ_Äã_>+öö ÄCÐzA*+/OoßîZÐ¢v ärUAhÐ+vaZ>©bbuCZhöC*ä*aö. oäUýrZo>ÐÄZ~,+AAbäÖCC|Üa~ÃoýaãUBÐ.r.ÜZZ,î_<.ý|_ÐzßZoOZrª', N',@CO>î<räýo¢UÜ|+u~b+AAh.©O_bî/rª _îÜvÐb~ößÄ|~Ã~@ªßb¢@Ðr*,Ä>z*,Z.Ü_zÄäßB>@>.u |ßýªA<B,|bª*|+|uÐu_@ZßA+ä£ßÄ_BAzýaßå>Ã', N'Oar:Ä*z.@ZÜväÄbÜÜªÐ,©+ã<AZ+__ÃÐbî£ v~~©@ ,rvzÜ/vübBîüðUßOÐ..ýÜäã|Ð*u+ÜrrðubvO~oväB|>~ßbü,@Bäov@|_*Ä|.rîräåüÐäü¢ª|o|îr+UÐ©,ßã©*b©Ã>Ðßä~ä£r+£Ãä*B/î>ZýÃ¢*oBýîA,b@@ßZA+ußÐüZðÄßÐ|åzZ,ðObbýbh|ÐåÃãvhäCä|zÐrCÖzÃÜð@/_C~Öah :Özüu£bäZ:£|üªÃza_Oß/Ð+roªCOÖBbva_£Bv*>|båauC*|ãäß~rãzåÄ,Cßª_îCr+,|/ö~b©A¢Ü|ÄhbßbZzü>å/Ã£öÄh_îAhÜßãÐßhåZÐîou_ ývAa|U~hÃ>üäÜaäîhhª¢ßîZ*aî,AuªÄro ö:C_ Ü|¢C*Öð*å.ãÜA¢î@Bü©ý£ã|ðBßÖÄ:b.ÜªßßboÜöªB|@ª,ß©~Ußã</_ÃðaÄA|ýrîßýbOußã*zAbåB ö_vCo<£uö*bCåb:Ö,ªå©Ä¢ £/ß', N'_:+£~ªboÖ/ßÐv~AÄCCîÜå|¢:ý/ª r¢©+br©>ü,@Zazýößo.C~¢@ åª|ýßî£oýÐöhÜ:©bî_ÐÐ~¢Ü~|Ã©ãA>Ö~B*>:¢ÃÃbÐ*ZZ*äO >*rAåßÐUãü.CUßh>ü+A/ür/uäÃýu*<UÃ@ *©ZÄÄzªU¢£v|ßðuÜzüßÐ+zîýOOß@å*ÐÐªî<:rv©ÐãÄ|ÜöZåba|OvåÜüAÜZ@@>b*Zð_öî/Ö~AzBî£ÐCbåzÐ> o:ããö/ÃðÖhUÖ+Z', N' ü£Uýhößäzz£U¢rÜ|AC:ÃCüýb©ZoãoZ/v©_ bv A@bý¢Cb©Cð:O O.Ãua£O äßöÃAä,|båÃzÃBö¢h*åaýoU,ýövãü  /baoÄCZ~U£BbbÜä£B<bb©Üo*Ã__ÄBª_ªÐ|UUÃðu@äÖ¢+åCO¢hZO+îU£©î©<O||>£C>z>@ªüOU_uîhß:|+ð~ß/oÃ>î~*a/böZUüU+ßÃB@ aC*AÐ', N'rîß£ýÄrüßß~övöÐrªö©O|bÐ©ãb:/A :obUß@UzZ_Öv~aBov+v', N'Bb*@COCßðUÜö:*/îahuüz£h.Z>Ä~ÃßBoC~+bÃÄÃ*C>+Zý h©£Uvubü:/ööU¢:_Cho¢ãäü*rZ:äãßZä.ü+bð/_äBhhðhÖ<bî|+uC*>vÄzbüÜOC£oB@vv.¢Ãªz@ÃÐAu:ß,üZ/bÐb:B ¢+|bîA>Üburß>öÃ¢ßãß@Ãb~O,uãuC*:ßobazAuª£uUOU~~< Ð.ðu£|£_ß+©ÃrZ£ÃÖ,£.ð,~~UOz/î*~ßÐ><Ã/UO~.:@©|î:ö~<<Oöb,*ß*ü:vu£/ÜäöÃßüv@ubý<åðA Ä*ã.å.åÃBhåBvÄzrßbåÃüÖh¢uöüåbð<åz£Ðð+U_+ývª©bOv@Öß©ßb +ürrOö/ý¢O©ýÜªü,ß|@>ÃÃh<|b©/hvU©ãüvÜÄObªðý£/aB,£|+üCv_¢©ßÐ£hvaä*ðýh')
INSERT [#OwnerContactInfos] ([ContactInfoId], [Email], [WorkPhone], [StreetAddress], [City], [State], [MainZip], [ExtendedZip]) VALUES (4, N'r©£>î~ÄbÄ<<£AåÜzh,z*ªöý_ð*ðhZ>aö~z*Ã ª*öðªBUO<UCBbß~ã:ÐU©B~|©|Üu:@bÄ@bü@A*.ýãÐ+/CßAUÃ.Bh|CAuaßZ+ãåÜ£.@~AbZ|<bÖ,ðÐo:ã@@@b.ðh.AÐ>UbAhüöÐA~Uao.ä¢bßÄB>ä*:ãz*Ðu<+oý Üz.aª/üaCäª>*Br', NULL, N'ÃöU¢åªrßªu>_ubAã£_rbuZh>Ð>ã üU_.¢b:bBh.ßßÜßÐ>~¢_<@:£ªÜB.ßÖî,Üåä*ÃbðoªýÐ>C,v/:,ýoäzä/ßåßãöAU*UAbðî<b.©<ð>ßö@u@£ü.ÖÐ@:A/|ßvbr|Ð,vB* _ý |>,,*£ã*~ zbß~>>~î<îUo~v*Ö,Ã/åO', NULL, N'OUhb©ÜÄ:©Öî>îßÖ~Ö.bbB.îCA~UoðÄhObÖ.v*@UÃ', N'<ã+ãäªöãCÜbðüÜ AAb_|@üªÐ/o,<UUO*åzðbÖrÐ£rB~+ChÜ¢ö <oo¢v@U£,Ãu:ªå~Äb©ãvZß å*/+UªZ,Ö_îÖaOUb.ßBßb.Ð>ÄÜ,BCß©b£ßvvãhýACZ<ðÃC~@ßª./OAZ<¢ð©:urZöääBOîýuZßbÃÖößß~OäZ>,Uä¢_ß£î:_ßða@bßa,ªß+hªC @@ßCbv*îÜ|¢a<£C  ßZvÜÖ.Ðý~ßv<hv¢ÖzaZãÜ+v:auü:ß_Ð>>bz ã|ÜCbh><B/£zýUo£ß>|/AßÖ@/ZOAbÖýbözîÐuoÜ¢åÜ*~~©:|+B©ý<äZaßU©ßü<.*:|*Ã /.©Ðð|U<OöÜßª£übýªBaîöü>ü©.+ahazÖuöÃav|ZA_ªzåü*ß:v:üUÃ/</oÖUoå:bö:b,ÄZbÄAßü* ©ªa ~îÖ©AbbÜ:Uã@zCob|', N'îüBOOÖð/ÄboÜ|Öð|ýÄ,î.aªUAuOÐÃßvÜzOB*<Ä|îCª*Äz:ZÜ+|o+O<aî+ãoü|OÖîß+<,£~bvh<Üß£îö|ö.+h£îªãorý£|ª ðã/îðýîhß<îuÃÃuU<î~Aoß¢vîÜ£u/~/Ä_häöuýa~AZ~£Ö©îhðö@Ðý|OCuOýo*©ÃrAO©_ßZA<îýýåuü@uä>z>bÄÐýüÐÐbaãU+~,¢|a+ £ðbßý~ZåU£Özã*rCz:Ã|b')
INSERT [#OwnerContactInfos] ([ContactInfoId], [Email], [WorkPhone], [StreetAddress], [City], [State], [MainZip], [ExtendedZip]) VALUES (5, N':bÖªZã*öu<O©|AåÐ,Ãä>,U_,a,v CzßUBð~ÖuüýbîÄ_ß©Ã¢B.ÄZåubªu_+ªå~Aãð©ö|£/h>/Ãßª©ÄÄÄzÜoî,aðaA +uä+C:äå¢hÐhü©ZräO.:ð@ß©ZC.C/îÖð_zbð£b~*Ðßb|üOß>¢roÐª|.ä,ObýbÖ/Ä,ß~£~+|ÜªbÐC/UÃBZð>UÖazýîu@:ý~zü**Ooªoîv¢+C|ßZ.o>ýÄCvrü+>ªÐ+ãBßAÐÜÖbb@>hauª,åÖo¢ß*öaåOÄo,ß©ý._._~Bßb@*bý<|CAª~<Ð/C~>hÜh|bªÃvã>Ð<ßbßßöBB:vü_zbÃ_ß ö|Zöð.ßövað£ßhýã£r|ÜÃß*AhuoUã<+bîðîö/@î£aã©Boß:ß_</<ýa£hrvßÄU|CÜ¢äÖîUb,CzîÜãCUä Ü*Ã*bîbåvÖÐOÄr./Äî<zb_|>bzobUßAü,<öîäÜÐÐßå£> u.C> CuÜ|ZuÜrvÐÐý£:ÐÖÜuv@|öOÐ@OîbOäîb~îÃßrãrÄC©>ýaßuOÖ|:OäýÖObAAC ÃA.ß©.ÖÖð|ðA_,î/OÐ', N'Äzª,Ðrªh@Öoßb+/£Bo ,<.ãý@hzªäÐäz O¢+.aB:ßªUo.CüOöv a, :Z<¢ä~îývÜîã<îÐbUBbå~åO©~,Oh£ª.ãðýÄ*îÜh>|ß <v~£b/åaUö>Ö~CßA:,Ü*©*bî<O,o:ö~zÖ_Ã.aÖhÃÖ~oî.UhUBÖ©Bß<©hr©oåbÄ|z*rýb.,AÄvb~Bro<UAhbÄv<@Zå|u ªU£Aaä_uUBßÄÄüßÄå|ðArîãî£äAå©ÄOäoÃCA/_b ¢vaö +ýz©ß,z¢rhÃ,Ö,oã©__ðü_aãßZ*<ª,ðîî~ZåÄråzäUü|/¢Ãoð>üÄ<A+ãü:ä@bßAßrß>OCvC:©ubÄ|:.Oªr_B.u/är/îU**vbuo*¢C@b©+@bUü£v>bÃðävð+ý+¢ü©*äCzüa>,CÜbboåÄîoOî|O.r£..ÜZÜ*v<|+z@,ÐZühä,v¢ãÄ+r<ãOÐ,<ö©:å|Uu¢Ö@~@Ü~r@/îö. aöa~Ä,bz@BÖoå*ãöãÐÐðU£ÖbzðÜaö,vßüoðÐ/zäãhzu.ß+åU', N'~Ou>b+OüvÜªb*CßürÄ.C>@ååÜð/ uöb_£,ßB~:B|_<bbuböB~äooßubbz ÄhöãÐzÐb.üzÐ+Ð¢ã~a<ß©Äý*Ö*ßåÄ.zÖÜUÐö¢ÃÖÖ*o£üÜä</@ü£b~@©aoªB+@ã*ý©_öhýÃuåvÄ/>ba|åä**ÐÖUOv~ÐÜAª+U@ß£zUî*/ãÖÜßåboöoAr>o>ªU.+Ã_vðOvî<ðÃ.Ã ý>î@ýý:zO/ß¢ý,äbar.îärßOð~@å,o:@:ÄrýUü¢Uöb>B:ZOåA,î>aß:ßÃÄª,:|îAüZoZOäßb~oößÃÄÐ¢,£äaUü@£v.z_+uÜ,o:COh', N'O~äîãä£BüýýaßÄîßaÖ>aÖ|Z©*+_¢U~_<<ö©/+b©Üª  _,rü©v©C.¢îðªZr|<zaä+/>@åA<.öBÐ,/uÐvh.å+|<hä~AbhO>O<~<üBîÃb¢ßvozî>>,_ð.b/£Zîz¢öãü u:îvßöv¢+: U~zzßüB@©~vBhAörbßvÖu£:AÄ.£Ã|hð_öÐObýª>©ã zÖö*ÜuäÖrãBÄB~Cv~. ,ü@©Äh,ãäu:Zªîî*Äb*¢.ßßðb:U¢~öÄaB /*ÜbUu+,a_Ö_<ßOU.rUÜî~oªªvîÜUî>:Ov:||¢ãhUAßvO@ü// _>Öz/ärO<ü ªhCAZ*u*¢ÃÜ*C_åÖå.bz: î*ýÃåãª++oäß¢åÄUåoÐBß£Öo£Ü_BzÜ.©åZuÖAª<', N'v ¢<zîýb£_î|+@vªãABZ>bªÖrð*£üuv©+*_zC©*å|b£U>B@ÖühO~,BUrÐåZ,_ B/vUraýö<OÃrÄ/.*UB.~ÃßAÃZvB ~U,ü Uöuüvî>Üa£aåO+ßß+¢Oüü¢v,|öðð£+UßÐå<>O+ýrðßÃoAßBîZ@rªrý+ªÃã.zü~h¢.rozå ¢ý£ýboöC+ÖäA£CO£~Ð>åß*üÃu*©bÖß¢Üaüîuöuðu£|:£/öða:Ö¢/ßa@ZAöÖÄ<î*_CBA©uüÃ+ßvUuåzUZ.©~ýrröa:ªOa:rÐÐuð_*ðvÜ>bU¢_ÖZ.ÄÃ<zß **îhAu¢ÜÃîª_@aöÜî+hîBðö~.UOr<zýîð.ä|¢rîAhÐ+*AaUv:ößCoðu/, ÐÜb/Z,îÃªBo,hÄ<CÖ*_~|.aîÐ_@åÐaO.Oz<CýÄ/ý: ,', N'bßb£boåîßÖzÖhÃÖäAöªðÐrãu*hAð_huaäß©Ð/uÃãz@*_o*ãaÃC.+Üu+äö', N'ðhü*©@zî*Aa/ßvÜªrãzbýzAu.ÄäU¢.ßß~ðÜÄ*OÃaä:<ü©ÃªBýªOa£ÜzªBÐðö©bvöUuãå>bvAoZr_ßb|ÐCÖUaUÜhßªoªªUÐ*ßO_Ð/ãÄªð*hã*|ÜÜAorãîßvÃ:Bbv :©Ü/ö.*Z hå,oz©BöÜuã+~/bå/ýðãv')
INSERT [#OwnerContactInfos] ([ContactInfoId], [Email], [WorkPhone], [StreetAddress], [City], [State], [MainZip], [ExtendedZip]) VALUES (6, N':v:Ã*..o.>ÃAz:Ö>~ª@zß/üåuÐ_>//:bÜBöåäýÐ@+b.åå.Ö/Ã< UÖbýaz|<¢äîÐýA¢vr@ä£ªäýðÐABýßüCßöCuzÐCö~Z@C©: .ãvh_ðßßÄ+ÜCUür~©£ÐBÜåß _B~Z+BÐ :.ªUÐ£.raC,ã_ªäü+Äã:ð+__aöª:*_:¢¢:©ß*öãau_b£<ýÜöuªî£UZ,båbUßbüä<©ÜrBðbª/äãu+zåbAo©UÐ£ß+*b~öZÄAövÃªýÄÜÃaOý<ß,bAÜýau£+Ðhãä,ãA*B ÄßåöÃÃå*U:©bÐoaýZAßüZ>,~_|ÜC ö££B©Öü>C BB£üOÄ©,©ä.|a|hAu.z.hðÜAzßaüb.A<OÃÄä£@uääoBvî+¢Bö¢@_OÃOo ª©*ÃÄ Ußð+zãv,ÃO:|üß>CðOða/å>Z>>_@Aäåooãä:ýýzUÃuã+Ðhu*ã<Ð', N'Ãå* £åßzÐ+ãzZ,hübhðä© rbh_*ZrÃ_CÄ,:ö>C B>ööäýb:Ðb+Ü*ßZB+ÃzrB_@BAßÄ~åör:ðßß*Ð+ bbB¢ußðrz.zbvrðU|ýãîbZ:|Üåðä_üö*ý*vCaîZvCª,O~åðva|v+å:ZÜª@ã îb©*<öbððßu©Ã|Ü:åZýîö£äüÄ|<_b<@<bö+bðbÃ¢ßa r*ª©z©ßåz_ö,UrÄ¢î<_ä/ZÖ~ÐrÄ_ßª*Cöz£¢CîoäUªröh|Cär.ÜoUoåð~aðCüvbÄOã/ð_B@a Ãªãbß:Obä*,ßräÜ/ÐACýbîü_<ÖZhO CîÜã||Aãß_ÖABð£îýý£>ßb.aZß¢zªAobî_u>A+Zu,åU|uaßbå|Üz<Ã>A>Ö_ÖðÐUÖ+Äh|Ãoå¢Ã.Cðããßva~/Avã|rArývß@z£¢<O@åu*ÄÃo¢b|~r', N'ZÖ*zu>@hð_>o¢ßoC:Z*£v,ÜUÄ Cu@£:ý Z@Öö*.ý£,öÃã_b*:ðCZäaÄîýCªå,zªZB@ã Z,OCå*ýä/B,ovÖÖª@oh ~¢ÜãÖ_ª*Ü©ª ÄÐbbÖa.>aã:åÐ/ª¢|aÖð,@Ö£ª*+*Ö£bÖ>b/@ÃÃã@Ð¢zÐoåCzðýßÜð©ðrý.:.£bÐ+ÖÄ,£ýbðåourÖrA<Ðub.ÜäzãßhU_vU/å©åðü:<:zãbðªÃ<OýüðÄO~£ü@:ä~@Ö_©¢Ä @ävbAUÜÖ~ÐöÃh*öv~ @vzað.@~îð|z+Zå+özÄîî<,ßbU~ÖÐu|<ü C:a@öä_/ ãrv*£ßÜ.£|©.Aä<ö©ãÖäÃ+b*bo/*/*ã+<o|¢ßåðöößðîZîB~_~ZaÜ|£<Cðz>+/ýZÄzöåzähÜðßðå/obhbãßãbã~î>b>å~.~@ß£rýörZ©/>Bü£ÜAöäo/@¢hråÐb+u*vr>U.bßübOä|h.~ÐªUÐA|a.übîÃ/zUîãbzOZ~ÐUÄUüÜý~ö<Ð¢hCß¢rßUÜ@O>*+©ßva@ßuZoåaaß~u/,B.üßo', N'rä@ð>b|Ð*åzüÄ|/£> ýUÜª,ra*hbB,_>Ãoß©ZZÜBzZ©îZvÄã©Ö:<Oî ~:zîo', N'£ö ßv|ÖÜ+r_åð>ö<ðÖ¢z. öb ßßUrü+¢Oã¢Ä~<ýoBå,UöZüã¢ü,o@¢bzav<<Ð@|,ãîÄBäUî:Ö<u@öÃCAßýªU~uC_Ö Ãzý>.~za.A|åü,bö,A._* o:©Öî ßªÜO @_~<Ub¢uövb:ðauO£ßBhuü+o@ã©©ýrßv,|/ã<:hüÜ .bbýÖÐÃhoßÃAB_O.ö.,üuÐ:ßb*|+ÄO©C+å|zb_Bðüãba|+:å£b@Ð|ßBÃ~vð,oz/üC~©uýä*. <ý+Ã/UÄîÃ', N'©ö_|ÜäÐßªÃÖ£,bîýÃUåÜÄC,BßÐahAu><~ÜzoAC/Ch@ß£Ð*üª_Ua£ßöö||:Öbä~ß,rvb¢ÜÖü~ßÜ¢OzÖ*:Ö,ªÜrãÃ£ãÖ~|*+U|Ð,/<v*ãÜZ.ãAzÜUßCZz<üaO*~/Üao,u:@b/ðzåO,|ã<Üa© b>Ü:bbÜ~ÄuBîã|ß¢îr+>U  >UCððh_, îBÄ@+ðO,Ð<ÄÃuý*ßÐu~~¢oBÐOZuöhã:OßÖýÐäzUüaýÖÖª¢vUÖBOðüßÄÖaý bÄuUîA_å<aªü/üª~£,*h|AüOb©_uabuÐäýovÜÖäÐbÐßUhCÖö*ub+©äÖbÐArU> ä.ÜåB:ß*ßî@ªbßuÄbðî~äßýäbauB@Ð_¢UÐå+<UAz', N'/OãbÜöãaýZ î:b/ðãBUÄãb*î+ öãUßåª/å+ßAauvv>ÖUz vßö|.öb/o+ßö©/aýÖO,Ð<~Ã¢åßäü/ zC~bbCîÃ+ö@ýÐýýb ÜvZÐ|ã|ãÃC ©ªbBaßbÄ*¢+åBbß:r<äÄ¢ß>bÄo~ÖvAu+a~AÜä.ýððZÐÄ.+r©Ö,+*ã,ÃäBa,ãuz: .ýä>AA~Ü>ðÜ bÜ© üðýä©ä_ªåoÃäAhÃªã.z.>Ã¢Ðß~ãî¢+ßz*båzz~©_:Uªå*ÄÖ+Uo>©<UãvZvbba£ý@++@üU,zvÐÖüUbÖ+Bß<£<åhßBuªab@~_ßvB>ürßCãäuðÐZ~r_îöv_ý>£U_.Ð£,UÐouoüýU.îA*ß¢£rBß£.<o~zýßÜ/v/Üb<:ª>Ö>CªuZ<bUOoåaA/ªbUðBoãu r_ýüCüå*ah:~ÃbOO©@bß£å/uýoüU_>*ðZÜ,©Z@ðvzßää~ãðAbÐ/¢Ö.ÃzCÜüüý¢ß@ß@zî£>îh~ß@ãA|£huörßbÄoåBB|>_üZ©ý bb_A+_ßzAüåãZ_hßÐb/a:B+/ß|ö,~¢@ääÃ@~ªr|£')
/****** Object:  Table [#Offices]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Offices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfficeNumber] [int] NOT NULL,
	[FloorNumber] [smallint] NOT NULL,
	[BuildingName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[City] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[State] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsWindowOffice] [bit] NOT NULL,
 CONSTRAINT [PK_Offices__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Offices] ON
INSERT [#Offices] ([Id], [OfficeNumber], [FloorNumber], [BuildingName], [City], [State], [IsWindowOffice]) VALUES (1, 1231, 1, N'37', N'Redmond', N'WA', 1)
INSERT [#Offices] ([Id], [OfficeNumber], [FloorNumber], [BuildingName], [City], [State], [IsWindowOffice]) VALUES (2, 23, 3, N'agaglha', N'Seattle', N'WA', 0)
INSERT [#Offices] ([Id], [OfficeNumber], [FloorNumber], [BuildingName], [City], [State], [IsWindowOffice]) VALUES (3, 46226, 1, N'37', N'Redmond', N'WA', 0)
SET IDENTITY_INSERT [#Offices] OFF
/****** Object:  Table [#NonDefaultMappings]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#NonDefaultMappings](
	[c1_int] [int] IDENTITY(100,2) NOT NULL,
	[c_int_AS_decimal] [decimal](28, 4) NOT NULL,
	[c_int_AS_numeric] [numeric](28, 4) NULL,
	[c_int_AS_float] [float] NOT NULL,
	[c_int_AS_money] [money] NULL,
	[c_int_AS_bigint] [bigint] NOT NULL,
	[c_smallint_AS_int] [int] NULL,
	[c_smallint_AS_decimal] [decimal](28, 4) NOT NULL,
	[c_smallint_AS_numeric] [numeric](28, 4) NULL,
	[c_smallint_AS_real] [real] NOT NULL,
	[c_smallint_AS_float] [float] NULL,
	[c_smallint_AS_money] [money] NOT NULL,
	[c_smallint_AS_smallmoney] [smallmoney] NULL,
	[c_smallint_AS_bigint] [bigint] NOT NULL,
	[c_tinyint_AS_int] [int] NULL,
	[c_tinyint_AS_smallint] [smallint] NOT NULL,
	[c_tinyint_AS_decimal] [decimal](28, 4) NULL,
	[c_tinyint_AS_numeric] [numeric](28, 4) NOT NULL,
	[c_tinyint_AS_real] [real] NULL,
	[c_tinyint_AS_float] [float] NOT NULL,
	[c_tinyint_AS_money] [money] NULL,
	[c_tinyint_AS_smallmoney] [smallmoney] NOT NULL,
	[c_tinyint_AS_bigint] [bigint] NULL,
	[c_smalldatetime_AS_datetime] [datetime] NOT NULL,
	[c_varchar_AS_nvarchar] [nvarchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c_char_AS_nchar] [nchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c_nvarchar_AS_ntext] [ntext] COLLATE Latin1_General_CI_AS NULL,
	[c_bigint_AS_decimal] [decimal](28, 4) NOT NULL,
	[c_bigint_AS_numeric] [numeric](28, 4) NULL,
 CONSTRAINT [PK__NonDefaultMappin__7A9C383C__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[c1_int] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#NonDefaultMappings] ON
INSERT [#NonDefaultMappings] ([c1_int], [c_int_AS_decimal], [c_int_AS_numeric], [c_int_AS_float], [c_int_AS_money], [c_int_AS_bigint], [c_smallint_AS_int], [c_smallint_AS_decimal], [c_smallint_AS_numeric], [c_smallint_AS_real], [c_smallint_AS_float], [c_smallint_AS_money], [c_smallint_AS_smallmoney], [c_smallint_AS_bigint], [c_tinyint_AS_int], [c_tinyint_AS_smallint], [c_tinyint_AS_decimal], [c_tinyint_AS_numeric], [c_tinyint_AS_real], [c_tinyint_AS_float], [c_tinyint_AS_money], [c_tinyint_AS_smallmoney], [c_tinyint_AS_bigint], [c_smalldatetime_AS_datetime], [c_varchar_AS_nvarchar], [c_char_AS_nchar], [c_nvarchar_AS_ntext], [c_bigint_AS_decimal], [c_bigint_AS_numeric]) VALUES (100, CAST(280673076.0000 AS Decimal(28, 4)), CAST(-1882090438.0000 AS Numeric(28, 4)), -1, NULL, 1414793172, 32767, CAST(28443.0000 AS Decimal(28, 4)), CAST(12902.0000 AS Numeric(28, 4)), 0, 6452, -4254.0000, 32767.0000, -13627, 128, 111, CAST(69.0000 AS Decimal(28, 4)), CAST(11.0000 AS Numeric(28, 4)), 0, 249, 185.0000, 55.0000, 157, CAST(0x00002CA9000C5C10 AS DateTime), N'hCh.u¢Z/+v©OªäÃÐÐð,ª:ÜÜoUZ~aý> @/býbz@AßÜ©£ÄB_ß++åzrãåß**üOöbv:äöhÖ|ä¢/uå~ãÜhbbÄ<u£BrCÃ_U£Ä *BÖ>ußÖößU>Ð:bzvbauZî:<ãª+©ß:rzrðCå', N' |*ZCîUÄî£Cß.>U/./åªðåäCö>~ðZß r@:Ä/åýß|uhÄÄ|abÃbÜ>ÃýOî:Äu*ª z.bzîZüßß_.¢ +ßOh|ð:åä@z£r _vÐ££OuðÄ¢.£ÄbîvuBb©ßCv|©/o/:îÜ£Äü._bîÜ:B:b_Ððu+ýªZuª_@/ýoýuaß<ßäÖöÖ£+r  ~~*AÄãÖrÜ:ãª_bO/ü_îð|/ZýUÃýý<Ðª.öåAöO,v UaU*¢uzÄÖz©ßZ.ÜhZ*ßAäÖÐbã/££auU~v~©|rö+A+vuobÜC_Cå©ãªU<<vr>O|£Ã_bB|ÄÐuö ýÜ©~_~ab C_h_.üABrZa/O<Ä~©böu.ßîö ãã|îð©O:ä/C|ý£h@ª@ððåßzöýä>bãîz¢Or|h:rv¢ßý A~aÃ.ßÃßZýa:b_ªîbohªöh>+ÜBýb,vzCå:v+üObUhBA£oÖ>ªäzu:Ã+,îB£î ß£.aÃî|~vU                                                                                            ', N'bå<ãBäua~,rBbã.@b*v ZÖ.<©aöUrð ª+*+ßî_ßU.åÐ// üã>b@îýª~ :ýobîüß©bªÐÃ:ª¢@_Aî,aýb*ABÖo,.¢ÖÖb*î+Ð :bb+ã~v+å/ý+öÄOz<ößbßr©v£üüö.vÄoßÃ/*öªrÖÜÄäÄ.b©b~Ðäö|Ã_U,ßÐäª ü¢Äã_vvã+äÖbvb*öä~zð*Ã,Ü~hhröhå h/ÖUa|>.ßÐ*aO>.<î/öh|ÃüvuðU,Cbåuü.îãzBöZBåCª +~BÐ©Z>rÐÜ::>AbzBzÐãhbÜª|oß>£b©|*ðã*o©/Ü+au¢:,ßbäýý|,Ür ß<O@<a|ÃBåB|OÐo/åü<hßhoO*Ð¢£bCO~Ð>_Ã>ðZ+/__|ZUåhã_Ã/Zb@© ýoÖ,îªýÄ>ã©r<öü©BÃvª~îCã>ß@.ÐZ©üä*>Ö@ý_ð|ãÜ.rö+ªÐÖr+UbÐBbîåbB|ýroÃ C/roCo*AÖCÄ¢|:ð©ýBzÐßª.Ä¢B:ãýä|', CAST(566457609.0000 AS Decimal(28, 4)), CAST(-9223372036854775808.0000 AS Numeric(28, 4)))
INSERT [#NonDefaultMappings] ([c1_int], [c_int_AS_decimal], [c_int_AS_numeric], [c_int_AS_float], [c_int_AS_money], [c_int_AS_bigint], [c_smallint_AS_int], [c_smallint_AS_decimal], [c_smallint_AS_numeric], [c_smallint_AS_real], [c_smallint_AS_float], [c_smallint_AS_money], [c_smallint_AS_smallmoney], [c_smallint_AS_bigint], [c_tinyint_AS_int], [c_tinyint_AS_smallint], [c_tinyint_AS_decimal], [c_tinyint_AS_numeric], [c_tinyint_AS_real], [c_tinyint_AS_float], [c_tinyint_AS_money], [c_tinyint_AS_smallmoney], [c_tinyint_AS_bigint], [c_smalldatetime_AS_datetime], [c_varchar_AS_nvarchar], [c_char_AS_nchar], [c_nvarchar_AS_ntext], [c_bigint_AS_decimal], [c_bigint_AS_numeric]) VALUES (102, CAST(-671342354.0000 AS Decimal(28, 4)), CAST(2147483647.0000 AS Numeric(28, 4)), -1883156520, 1298338223.0000, -902842942, 32767, CAST(-32162.0000 AS Decimal(28, 4)), CAST(-5507.0000 AS Numeric(28, 4)), 29105, 16308, 10345.0000, -1.0000, 25678, 100, 249, NULL, CAST(174.0000 AS Numeric(28, 4)), 142, 91, 255.0000, 55.0000, NULL, CAST(0x0000A551006B6430 AS DateTime), N'åZã@åðußvßðî ýåuîß/ü@*baÜ>vÐü uAZzaU|OB uåýÖb:ªð Ä@>r/ß@UßÐ+©>ãö~îªaü££ÄÃBÐ¢*zö å ,UßÐßä<>ýÐ:Ð©ÐbbOoä~üÃbßßBÄOoöä.< B~CäbÐ_<£ðvü.a~zhß:z+ÄOaðÐoZ båbU*¢:ðzBCA<Ah_ä©ÄªäßböOü,.Cð*~v', N'AuÐb+h+ßÄ*vå©A<bb/Ã<UßBb,uAß+Ö@Ü*:CzC~ :>ý*~ðbÄ+¢äZo|©*uC©ÃoC_/üã<_uðOö<CUüU:@hz@£r@<+u©Bår:îO*ö|,Ð/@<::ðßb ÃBÜª_+¢: ð><våCbý<|ðîa©bÜÃÜArÖ~©zåüaC~ð@_å¢£îvO£*.:/uC/©.Ããã~ªU| ßo+îCð,|b,a,Ao>@h©ßZh/r:z~.ÖBðð£b bBßÖU<båOÄ.Z,_ z:ö/ZO,öðª_@¢:C:+ ÄB,Ð+/ uvv<Aarå/ß<ýrÜh©håoÄü/.bvböÃ/ð~b>Cbðh<å~ ¢r©£:o||.UãuÐ*                                                                                                                                                                                                                  ', N'ruüb>/>r,>bÄr_©a|+ uÐ,ßî+CA©ÐãÜÜvBðvî>ãrÃ:vübã+ä~ã£*a_Bß.bÄ,.ã<_Äz/C¢.Bb_h_vhãß@ÖoBîr/ÖÐýCäãß/ßäýÄvÜ.vUoÖ~ßåaß©|UO¢UÖ|*Z_ÄC>|UvB*©Z/~bC>îðoªªåÄo:Z~¢ß_C.+ýuÃhÐª>zÐ_*_uýrbh ã.Ð©©aöbv©£vß|äbªß/*Oz£zå*+ªuZ+|ÃuZa¢z_ÃOÃ*ã©:ÐÜÃo,,¢Üo@Ö/Ü £*ßüZb~|Zã:ZvÖO<üß¢/ª~îbîýöîbÖUr@uAãå+_~äo>äÄCCäª©|ðß*Äb/ÐbZCÃ>@~vÖbÄ ªªZA~/Bb+ßåbz,r ,O@ý.hZU<o©üv:*bovbü£ÖÜvü++Z.A,C vä~@Äð<zA|.ÜCÜð/ßðAzU©¢,UaAÖaã/Ü_£a~Ö@/*ª£ªr*>ãª:ã~C+ßB~uOZÄîÄzrC£u<Ü ÃZ<©Ä~b/bÐ', CAST(1269258048.0000 AS Decimal(28, 4)), NULL)
INSERT [#NonDefaultMappings] ([c1_int], [c_int_AS_decimal], [c_int_AS_numeric], [c_int_AS_float], [c_int_AS_money], [c_int_AS_bigint], [c_smallint_AS_int], [c_smallint_AS_decimal], [c_smallint_AS_numeric], [c_smallint_AS_real], [c_smallint_AS_float], [c_smallint_AS_money], [c_smallint_AS_smallmoney], [c_smallint_AS_bigint], [c_tinyint_AS_int], [c_tinyint_AS_smallint], [c_tinyint_AS_decimal], [c_tinyint_AS_numeric], [c_tinyint_AS_real], [c_tinyint_AS_float], [c_tinyint_AS_money], [c_tinyint_AS_smallmoney], [c_tinyint_AS_bigint], [c_smalldatetime_AS_datetime], [c_varchar_AS_nvarchar], [c_char_AS_nchar], [c_nvarchar_AS_ntext], [c_bigint_AS_decimal], [c_bigint_AS_numeric]) VALUES (104, CAST(-2147483648.0000 AS Decimal(28, 4)), CAST(0.0000 AS Numeric(28, 4)), 1, 1417914182.0000, 1115782007, -32768, CAST(-21434.0000 AS Decimal(28, 4)), CAST(-13669.0000 AS Numeric(28, 4)), -24240, -2750, 15839.0000, NULL, -1, 186, 196, CAST(255.0000 AS Decimal(28, 4)), CAST(1.0000 AS Numeric(28, 4)), 115, 28, 255.0000, 36.0000, 248, CAST(0x0000FFFF018B3BB0 AS DateTime), N' åBbr.rªª.>bAvß,ÐýÃå,/Z~ä..öhCoö@h.ß|Z_O|_.ßöhb<ýÖ.üvbäå~>OÜýU:<v_@åäýã::@ ãa<¢Ð öü,ÄO©<v@ubCÖrzBð_,ªrªýîü@ÖÄAäCå//AZbÜBäbª ÜÜ*Ã', N'ÖbðuC                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           ', N'ü>Ö©ðÖU£CO© oaAo:_|~vuCªß/ÄZÜO£ ZÃz|A.öZ+Ã©ªÃ+oýÐð<Ã||bÖ+hã>Aüäð@*A:Ä:CA~vr~|@ð/vÐ_äÄöÖÄ¢_rå~/U,ßÐb  ÃZvß/Cz+<rãr,Uz.au~~zãoUu:haßÜß:~C|/Zbvã/*o*+ÃOrðýß@>ÖaähA|ýüz|Ã,,ªZ©åhü', CAST(-473253532.0000 AS Decimal(28, 4)), CAST(-1.0000 AS Numeric(28, 4)))
SET IDENTITY_INSERT [#NonDefaultMappings] OFF
/****** Object:  Table [#NonDefaultFacets]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#NonDefaultFacets](
	[c1_int] [int] IDENTITY(100,2) NOT NULL,
	[c_decimal27_3_AS_decimal28_4] [decimal](28, 4) NULL,
	[c_decimal24_0_AS_decimal26_2] [decimal](26, 2) NULL,
	[c_numeric24_0_AS_numeric28_4] [numeric](28, 4) NULL,
	[c_numeric24_0_AS_numeric25_1] [numeric](25, 1) NULL,
	[c_varchar230_AS_varchar512] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c_varchar17_AS_varchar98] [varchar](98) COLLATE Latin1_General_CI_AS NULL,
	[c_varbinary60_AS_varbinary512] [varbinary](512) NULL,
	[c_varbinary31_AS_varbinary365] [varbinary](365) NULL,
	[c_varchar80_AS_nvarchar512] [nvarchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c_varchar185_AS_nvarchar285] [nvarchar](285) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__NonDefaultFacets__7C8480AE__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[c1_int] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#NonDefaultFacets] ON
INSERT [#NonDefaultFacets] ([c1_int], [c_decimal27_3_AS_decimal28_4], [c_decimal24_0_AS_decimal26_2], [c_numeric24_0_AS_numeric28_4], [c_numeric24_0_AS_numeric25_1], [c_varchar230_AS_varchar512], [c_varchar17_AS_varchar98], [c_varbinary60_AS_varbinary512], [c_varbinary31_AS_varbinary365], [c_varchar80_AS_nvarchar512], [c_varchar185_AS_nvarchar285]) VALUES (100, CAST(0.9000 AS Decimal(28, 4)), CAST(1.00 AS Decimal(26, 2)), CAST(0.0000 AS Numeric(28, 4)), CAST(-1.0 AS Numeric(25, 1)), N'ªoåýCäü_>a<@AÄ@bî_:höuÐh,ABßÖ¢/+*Öã©Bä:,_bÐobbb>,ýb/åh+,/ÖªbÖhßä~BOväãöABb~å', NULL, 0x9182B8F9FA, 0x, N'+/U*v+£,vä|öîß£_', N'*ß_>îã£ZC')
INSERT [#NonDefaultFacets] ([c1_int], [c_decimal27_3_AS_decimal28_4], [c_decimal24_0_AS_decimal26_2], [c_numeric24_0_AS_numeric28_4], [c_numeric24_0_AS_numeric25_1], [c_varchar230_AS_varchar512], [c_varchar17_AS_varchar98], [c_varbinary60_AS_varbinary512], [c_varbinary31_AS_varbinary365], [c_varchar80_AS_nvarchar512], [c_varchar185_AS_nvarchar285]) VALUES (102, CAST(0.4210 AS Decimal(28, 4)), CAST(1.00 AS Decimal(26, 2)), NULL, CAST(100000000000000000000000.0 AS Numeric(25, 1)), N'*,bÄZ+ßChZ¢Ãb>>ßh__:.hößU<ð*ühbÖuãU©+*:ßªOaüÄÄ©bî|/Ãrý:r:Z:ß::oauå:Uvã.ßBÖäUhýB@ßä/.Ã+|U+buBå© hAýÐßU/Übo£ª/hub:Oöý~AÐ ä uäuª î..Ärhvvv+b*vbÖCBhüÖîö©ÐCZ©ä>a||<ßÐOA.aðrð©¢>©<vBrCÄUã.üÖ©~ð:,ÖUß@å/z~bÃhb_üv', N':ß.Ö', 0xBD71FFBC9387B5F710E3EBF79F001E2DE61C6AD3C9EAF3B9956B83720EAB110C52B858236E7A8D68BCA5E08FBCB3E9BBF33167FD05F8, 0x7E4626280342E9, N'OÄbßß@A,*,~|ª|¢ä<r:+ª+zîÐbðb><ão', N'_Cß.:oÄbîÜäÃ@Ö rrÄUªßß,hBÐ~öîä_.>.')
INSERT [#NonDefaultFacets] ([c1_int], [c_decimal27_3_AS_decimal28_4], [c_decimal24_0_AS_decimal26_2], [c_numeric24_0_AS_numeric28_4], [c_numeric24_0_AS_numeric25_1], [c_varchar230_AS_varchar512], [c_varchar17_AS_varchar98], [c_varbinary60_AS_varbinary512], [c_varbinary31_AS_varbinary365], [c_varchar80_AS_nvarchar512], [c_varchar185_AS_nvarchar285]) VALUES (104, CAST(-100000000000000000000000.0000 AS Decimal(28, 4)), CAST(0.00 AS Decimal(26, 2)), CAST(1.0000 AS Numeric(28, 4)), CAST(-100000000000000000000000.0 AS Numeric(25, 1)), N'BÄzU<Zåhß:öýbå_bäã~ÖªbOzoAÄauÐÄB/ÖOö|å<£äýouäî:,,aüoÐrUA_ýB:~äðbÖä_ Ä~>ü*ðîªîö:v|z¢rÜ|+u@h.|ßabýbB<ý*hÃß@ýßßz¢h~_ãåvUÄZr> ß|OðO äî_ß<:î/¢ß©ÜÖ|ðîaßAöCßÖ*ßZ>a*C¢ªß:ö,B Bbð:£/A@/ð.î_*åuuboOarüåB>Ob', N'¢@ß©Ð', 0xEA, NULL, N'Zã|ª/~vÐ~UäUð îCA+hUü|Czå', N'¢ðr_ðãäÄ*,Z£ðî +bÖZå ãäýZab*ÐÐvÐ*Ã£ß,Ãö.b¢_ÄÖÄr ¢Ob_ªu_ßbrUîÖ>ü¢h_Zuv@ý+ßäßÄZhU~ßÖÄ_©ÄChoãÖO/A£|£î~r.übBr©ÖÐ,ß>ðü©Cýß/ö/.ob/ÜßöÜuvb,|AoßÄ¢ aü,Ð  ãuÖð£ÜrüoÜvî¢ .Ðü@')
SET IDENTITY_INSERT [#NonDefaultFacets] OFF
/****** Object:  Table [#Owners]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Owners](
	[Id] [int] NOT NULL,
	[FirstName] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[LastName] [int] NULL,
	[Alias] [varchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
 CONSTRAINT [PK__Owners__03317E3D__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (1, N'ÄCßÃu>Az>ÐbrÐ+,*|©_zA>u~h|vbÃ|O|Ã£¢rbÄãBÃ©vaýðz ãhÜ>vüUråu|©uýªð|>>bÐrZãß:¢+UuªrUör_ÜÃzÄ./zäoOäÄb /<.u~£äö©ß åü©ª*.ZÜboåOß©zå.Ã£ªðzîÜCÜA', 1195612311, N'>+_<£UßÖ')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (2, N'ýO|z¢_vÖ/Uar.|bå>|,ðu+zä©.hãuOåîAýzÄrßo<Bßv~aZüü.ÖÖU+¢Ã¢äÄ¢u./@v+CO.U*ÃÃ.ðã/BÐã+:O.:/£_,z©ã>Oðb£výuîC_Äbbbß/ßv@häUÖuAbãÄ|ÜC~>r><ãroC>övî:*,+ßBªAhÃ¢öOãªro> */h~å|*ªªC.¢ðýäÜÃ©:å<Ob/ß|_za~ööaªî< Ö>ß<>aðÄbÄüzbð_Ã¢äO*ÃÜzAA~ao<¢zb,/ðO©,ýOoÄÐä~aö£.U+ÖÐC¢ýOÜ:îýÖU/.åî¢a*AuåýOýb:/A¢+ÖUU_*vßöýîöbr.<Z:ÃB £broåÐð£ðÐ .vªýaOä_£ßb*CvßßÜ@ÖýÖözÐÄAB£ª*Oav~@ß~/:C~h:@|ÃBîbðoCoÐ|<üåZüBuöZ¢bB_Öau b£.ª>ÜoaöO|©Obã_abßÃh<a+roî@ª_hbÃßb@zöýrÐZAî£vC~r|_Ãr_r|¢b.*å£.,b>ü*ý ubb¢üä:BÐöA zAa/åo¢>åv_¢zB', -1371488625, N'Ü~å*:£||*C@z<z| +,ª¢ßÐ</ö@.zåýßU~£vÐ/Ã_ßzªZ,*öb ÖÖ>Uv><AC/,öuvÃ*zZÐßbßîÃãß r:BBý<B_h>|üBOu¢ýZª~bîðBbuUZöãß,<Ov~A .:CÄ|äÐz££ßrî,Urvz@ýAÖUßªZÃo>îð¢ª')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (3, N'h< *b*Ö~~:Öö.oZîübAz£uzBZo~ßÐÐO~U@BÐðü:¢ZO£*ä*Zýuýö||*uðoubîÜðääZoÃåb|üý¢,a<~öUðå~ªÐÜªAîZO|Ãî_ã+zÜbÄ©,ð£AüöO*Ð rýharzCB©hßªåývBb_£oßî_v>£öÄß©ã|_ã*ÄÖ_£br©,~äßb£Öää¢Ã~_z:ä>|+.ZýZUbb£|*_îªüh>ö,ÄªZüößuäüzAvîABÃÜ+,Z¢Ðýar> ÄBª,ß>:ßöb: Ob+bÐ¢Z/>ßðOÖB£ÜüªZÜ:<>Ü+îuÜ/Uhª brUv©U+îbßîo/£ÄãÃß ÜÃåÄ|~Äã.@ývÃzãÄå.>z*ðä+£ªªðv>öÖ.ßã£o>©©ö.ü+b|rä~öß.O@ýäß>böüÜ|î~Uãå<O_,Öã*ðö<zb,Ö* Ã@~îßüã£/ÄÃOãbßãÄz+,,.©*obî Oý|r.Uub£öb>*îüAhObåÃ<+b,üöväUÖÖb@:/o<ÖªãÐOÜUåb<:¢rýüa¢Ä', 1008457066, N'ör.O /ã/@ä£/<büz~ã>ð:ßuAÜßbåÖßÃ£ZZå_Ü~ð/rbðÜ__<ü©.A¢ _ähü+.@,ü©ÄoýhOO._ýðbÄ,å:>uzbr*ßb Ð/Ð~,O,AU+u¢_.zßUäAÃUã:U>åruzÖ îb©,Ö/ãhß_A:a©ãößaZö<Ö,Bîü>b>üOÄ>Az_åa_*')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (4, N'uhr+:~©ö,uAßäoÃ~>ÜöÜ¢ã+züü+zbß:ð_Ö@Uå_@î,|~öÃÃOhãz>Ü.£B+Üî,<u+å~ÖBvC£:ÄüvA+bv.¢vßu:ÖzÃ@å£o<ýhbÃAÃaOª/b>,ª_>,ZAB*vh©ýðhvb|+:<ýÐãu  ,vuAãÄÐboÖOhª¢bCÄý+ÄC_ÐCAb¢a.äÄZbö¢ãhUZüväü>î/Öß/äaããªuã_CýÜBÄ¢Zbåb©ýuü<Ä.:U~ur~zß~_äå:/ÄßzaU@ä_u©:a*Bãuå_C.oývzð,<Zhb/O/¢ürîüur_îaîÜOUuü*£zÖAZÐ_ýBãA~.oUv~ÐC£zðzðåUü*_Ðov Ü/h/,*U£ðä+oü>ä£>~>B bÐýßoAÖO:*ÃÜv¢ÐCãO+åý,böÜîÖCÃAh£oýÐÃhªU <::>Ã+Äåh_£', -2147483648, N' ©©bAUZ¢>.rAªvîAüUöb._ð|U,Oäýh,Ö@ü£bAb|ýAãübz©Ü/ýC|.:bßßÐCß¢b.£*A:>£¢ãýå©üA||åU|¢Öåå~ýz/Oã ðvððªr¢aãoC')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (5, N'CÃÐ<bUuãäUaz_ÜÜAob:Ü>b¢ÖA.*öä.,>ðÐÖåvß,öª<baðªb/ãý©B©b¢bbý:£åZý/ã>,ßaåÜ*©@o', 2147483647, N'ß. ðBåÐzãåBÃãv*Ðb >£o/uv.ür .håªb:oAzüOBÃ_£@Äzb>ýö:ã_ /,rb>hÃ,åÄý>ª+B /Bo,ö¢ÜÃ_CðÃ*<©bbÖaÜßu:A,z£ð_uÃ©,©>*Ãh,ßª@@,uýb.Aö.ä* >r£Cý£üão .vZÃüz+ð*rOzÄbß.UUî~ßßzbozÄ|hÖãÖ*rýöÄ+/Ð~>@oor>~Z~,Äö|@~öu@©ªßÜ~*.ü>bäÃäßOazzhðüßhv_*hßÜ©,a>ãÜhÜÄðßÐUý>.î_îÜ|uZßZ|ã:@UUuªOãÃäraAAu~+ OÄß¢ã£ßuAu:CrrZð©U<Ã~Cboß|Ã@åÐ ¢ð:CãAã©~.ý~')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (6, N'~£aÐB+ýC©Öb©£ßZüZ ZåßOÐb/îß|åv~_BÜUîÃßÜ**>UÄbO+A,¢h©Zb|,ððZr_.u,o@|å~+v<|ð_zöAav@ßOvhðUAuãýÜ>~ßðZã|_ª>:A*£B+@U/îîA©ÖUüvuZ_£Ð,>ýüC¢AB:<£A*oî<ßå>ý,Bä<îbÃz| vbäÄ¢ä©hUîr*/.,£,Bbß.,A£ Z¢>bbðÐ@hu~O¢@', 1725691548, N'ßb©/Bv©ªvÜðäAr,*uobOü<ßZrß<Ö~ýýÄÜ.äv¢O.ß+ :Öu*_üUoã£¢/£ U+UZÜðavÜrBª*CÄAÐvß¢abbãU>>.o*B Ü@AUöhA:zrüAvð¢C/ßî+¢a O ,Av.ªB/CÖãßB<bªU*zßBA~ å+ü+:ÄÄÖrãð|BÖAÄB_î>BßAüÄZhvÄ>bZßÄ,b¢Ðý,bB¢U_@>< ªÄ|hbUB*ý£o~/bÃÖ£Ö,*ýb+_>:ýoÜ.rß£öBß@oÐ:ZÃ£bßß¢Aö<îÖBC @*a¢¢Uö||:+ö>öîU<AðbZ/ZÐÃãüuZ@:Ã@ª~/>@Bu*hb¢:åhBAU.ÄobbBbZößvÄvbý©.å+:Ðöð')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (7, N'o+£vßÃåObBÐ@<|>*u~zö¢äa¢ß:UÐ|>:ß:zåÄ.ACB,_rüvhö äB@+¢ÖCz*a:*:oª|¢öbo©ÜöO:r_b£b*£>îä:üBoÃr~+,|Ü¢Uð£  bß£Bo î.rA£Z£ßä_Zh~@Ü+öB,_¢Ã~zoaÄA~ÄvÜz£OÐ>OðBÐÖîBöÜbß|Ãä|Ã¢Ãuãîå£|¢uoZ@ãö AäÖoZßåÜ>*+öî<åäÄb©öüîªzZ_C@Öß.öC<@ãB,,ßß/*_>Ä£o Z~:oa+hv:åÖÃ*aÃö/ß/öbÖz©äC__bß~åör:rüÜCB,|¢>¢h:.£ªbåurv_vÜzåã~u©zåÐo|<ü©Ä |>hßAÐ£Cå:ÐÃ©ÖððöO:uvß*ürÖ,zäO¢.oå¢Ü,:Ð~uÄÃÖöªuüu¢_Ür>bÖoOªZ *vÄßz_ÜCo_</åa_C', 1, N'@/bu¢ãu©ßåªrä:vå:ýð¢b/äbBb*z>@£|oaÖåvAbüBåüU_ ¢ý¢CBC£,_B|ýh|>îoZubbãÃ+ª.îü@O:ª_Ö/*Ã+/@ðÜ<© Zå><OÄÃA<ZÜZU@©uÃä*¢ü/,hhußãÃCZ*Abz_.<*:Ö~b<©b/ãÖzuîCA*ã>+*üîä *a.Uruhßu@îä++î<bÜüb+î+UaÃ<AAh+_îUÄ<Ußu¢¢@a,uoý<ªîZOöÜü:¢OU@ÖBÖbo*üC©.uU|*:äåýaãÃý/härªhZUãb|Äå@ÖöUäräßO©A¢ªzC~£o.ßÖ©ªuöÖßã/~oÃã<ðBöÐo¢ãrv<ã|aßO£rîhîC')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (8, N'ÐZC+A+ª>*ßh+a£îaÜü_,h+,Ðý©Ü+v_ð*büu:Avßã©ý<Üå/oaZî+ð~:a*å,++~uußäÜzb//', -1118370915, N'oîªä_>ÖB+oýÜªüä|¢å.U/Ö©îBhC~ãîãÖäÐã/Z@ßåßhä£åaÄb|ä*£Ö:Z,UÄÜ.+OU: UîåC/ZCb_.zåaAB/AAü|rý,//r~Ö.>£å. ¢Cßüh~u~rbý@u©Ãý hvåCU>/©uÄ+£AÖ.<Ã,üªbî, ¢ß*:ðýîb/O>¢@r,ª+zðääðoZ|A@vÄ+vUä_>:hýÐußZuävßö>a+uÜü@<Ã¢båå<£+bîUÖüÄð:+|Ãb|ÄzaÜbAU h>v*bßãbCãÐoðoöaAýÜbîärZ:îýzbBß:,Að.ü.åðB,vÄöî£_*aÖBUO.ZZýåOß@aAAZÖîU_AªBäü>~> |v/üA*Äruª¢hý_ÄC~,UÃ©oÐZßýhý,rîÃÜ£ ð+_üÃzb_Z¢,Ab,Ö,C*ß©.@oobrÐßAÃð+ ußÐA/|')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (9, N'ª:©/öbª¢ßß*üýð,u,î<~v@~ÃAÄäªa*©ß:Bßb<ÄBhCOuî~,*z,@¢@bÜ Bäü.Uua¢:C_@îC ÐÃß*:bzÃC¢*_r>ÄoÄ©ÖÃr>zäîC|ÖÜAå:ObßöÄÄzvßoäÜå©ßhÐüããh/hu+ö/*@_ðbÖî+hÃÜ/a*>åA¢_ýUz.AßrAzÐð|~ÜZüÐvUßAýZ©/ü@ß<£_B~ßürAB@CåÄ~¢rhO£Ä,Ö¢ßã@ýö£ýbv+_©OãýÖßBð£ÖCðz.©:zÜåCbåðzÐzüU,C/ã>oî/+h/ã¢<Cä©ðr~Zr©Z,v:ßv@U:ãã__zv.h>ubo*A<ªÜb¢ßB,üO/h/UÃ', -377816806, N'ýÜð||,>bbbC@ö+ÜäArå.*:å*CoßO:.buv+AÄÃr>uzbzîbîrb:ãbüü<¢@î~:,| ª©ýîU~ boãÄ©CZoüZrüÖÄª*bUÜÐ@ßCÐB£ý/£îo©ªÖ,©uvz+ +ðZv.ßªý ð£:ãßvbBªahä@bC_*ý ðCßö.,håß<Ãã+/ ý>rÖöCªC©zu<ääoÐbãZbrBÄA:©,U¢îoÖ.rb¢*ªbuzýo£rîZa£Ö¢aB+CîßozvÐ+>Öh:b©ß_,:ÜbÖröbã_,+ªåZo©AboßOZÜ>ö>,@ÐãªuC+ouA¢ðöÐzÖå+Üª:üßA~£ÄrOa|ßÃ|_CÄÜo<@hö£.ouBå@îhOåð_ößA|,oýu:ßu,rObCrZB Ah¢//¢z£B/¢ß¢aOðÖ~B*hÄr~ößîÃ<ßðåßª>ßo~hÜä*CaO.ÜOah+¢rÖO öb>ªÐ¢oz@Öaa>+Öo/ <ß~O,@AÄÃ*/ZÃA.OäBB@AUrß</,Ö©¢+rOäv,ÐÖ£ü|AößåUbýª @£bîZUªßÃühZAbýuBßÜa:B:CöaZ,îU+.o*CÄhv~aßðbÃãZýåß©')
INSERT [#Owners] ([Id], [FirstName], [LastName], [Alias]) VALUES (10, N'u~ä/röövöuå/~ÐC©ðvbßuÖu@aÖZ©A.|UChr~<Ã@r  b~b¢CBOzö|~za:**./£ZU_£¢z/__rUÖå>îÄ£:,£O>Ä:a©ßhÐuhZb_Ab>ð/ ©|¢CÄßÃßBÖÄåAÜ:ß@B£ü', 663351288, N'ü©*<ßrÄAbý¢äªÖrüabý>UvZbU|.Z:ß')
/****** Object:  Table [#Projects]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Projects](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__Projects__2F10007B__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Projects] ([Id], [Name]) VALUES (1, N'h,ÄOUAZ<@är:vv,,ðhªß¢<bOß¢/|*>våvîÐ|Ã@>ÜÃb~:Ðã_CCý@ýO>*£*@+¢ªÐCA~zr*_ßÃ,£ßB|î~/öb /*üvÃÜ|buOoäOüð_å/£O~zzh*+,ÜÖÖB._ZZÜbzª/ß:ª>uÜ¢h¢*ÄazîÐhbãð +Üz<BÐzå©o<|ßu,CÃßî~Ðvo,bbC£_hÄu,ZbahC.>:Ð:<åhrB>ª/Uö*ªB¢ð.aª£*@/vo@o+ý~+,Cýß©@îåAoöÄ<vbÃü,uÄ.åß,ðA¢ü©ðh¢äå_uªZö,@C~>,ßüýCãrýÖåü@ ~î£îðuBAzã~ÄßbÃ:ãOZzZýBb¢äßÐhäýðAîa+z<~î><Bü+vzÜ@@ðÃ©rAªß|vb,*å:ä©Ãðbzo*ª,ßßîCÃ+_bu¢ðzvßä.|Ðr>>hª¢CÄ~+Är+~Bv.rÃÃåî@ hv_|.äb¢Ð/hBå©uZýýÜO*aU uý.©Ðåãzüoa b|zÐ b<ö:ý+Öý.ÃîC*ýCar*./,îä~ãCÄ£år/©bãv_Zåu¢©Ã OöO¢Oî åuåZöoª+¢å£BBrUub>Ð|')
/****** Object:  Table [#Computers]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Computers](
	[MachineName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Manufacturer] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Model] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Computers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[MachineName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Computers] ([MachineName], [Manufacturer], [Model]) VALUES (N'christrolaptop01', N'Dell', N'XPS M140')
INSERT [#Computers] ([MachineName], [Manufacturer], [Model]) VALUES (N'christrotest02', N'Dell', N'Dimension 4352')
INSERT [#Computers] ([MachineName], [Manufacturer], [Model]) VALUES (N'christrotest03', N'Dell', N'Dimension 820')
/****** Object:  Table [#Testers]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Testers](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__Testers__36B12243__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Testers] ([Id], [Name]) VALUES (1, N'O_üZäÐCu/brÃb@.£îOrb|:ßbvß¢~C.hðÐÄ>.*+AC.ªÜbÐ,O / .+äbªÄÐîAbB@CaöZü+Aaz_hb£ü©Özä¢zoÜaÖb©BÜ')
INSERT [#Testers] ([Id], [Name]) VALUES (2, N' ýAå<ÜB:.ßZ.AäuÜ.£bü~o/ß* ýOrzZv ärobÜ<î©ä/ßaÐÖo_ZU/ÐÄãO_U@oÜîUC@ABo£bÐ£OhzÐÄªÜ¢|ã£ðbã@ªAAªßUzCÖª Zü¢ª><_~v~.C:ð:îªöv b©@>h¢:ð,äa_h, ßýü*O+:~ðrî~©**ÄO>Zv¢:h<åA¢O,~äübª£ð£ªböªb~zuAÜ|boßbãhÄ*ã<:oÖvr* ö@bÃ/b~.*£_åãUãUbhuÖßCðîzb')
INSERT [#Testers] ([Id], [Name]) VALUES (3, N'ðO åuüZ@Ää¢Bu¢><o:ýU¢îÃhü~>ß~o,*ð:>o ££äüz_r@h@r¢:Ðäbð>a>rð¢<:ö_<vOÜ')
INSERT [#Testers] ([Id], [Name]) VALUES (4, N'o@ßhvz*<ªßbzrh._výö<£CÐA,>+aÐUüÖbvZubö£r©A~B|ovßCöýOß£ÃAÜÜÐÖö*Ö+ãzUðãr< *Öß¢_£:uU|åßvßö.|v/vrîUzZuUvî üð:©ßbßb<h >>+:üBb+ã*~ÖrC:>>* ,r îªö ªüå~>ÜzöÃ,åãzCrîbü+Ov_+| ÐoÐhO~Ö@¢>b<Uä')
INSERT [#Testers] ([Id], [Name]) VALUES (5, N'U/b¢£ArrzUöUÜbbª vÖª|ß>ßC ÃªC.ãC|ÄaCzð©Äö_ßö£@Ð|:<oýCrå.ðv@  ªBå,ãÃbböZ,~A£Aý|h~ªCo£Uäý*ß~Ob,*+*:ð_ßã©>>rÖ,b_ä.*CöÐu:/£_ã U<ÖuaU©z~ba Ððª£:<<ðßüoãö:¢oîaZaý@/AÖüÖh.UZ+ Ürßªßýobî@Zzðv_¢>.|Üa*u<|î<Ä @|©ß>ovß_BaCðbýoÐßÃu@îßÄ~ /ýzAå@:Ö,ü')
INSERT [#Testers] ([Id], [Name]) VALUES (6, N'.CZßüz|©BZu ãbÜ>ß¢ÐöuOÐv>@r>©O©A_u.~öÐö@oðUoÖýrßÐööÜu©ZU+aZ/BbýîªrÜ¢©r|b©rßZö|@@ ,ÜÄuåßåðOrð/>.Bð __r|o@+åBaäýZß<ÖCÄzv©Ä.îÃ>ý¢¢OßÃ+ªAý ÖuU**ßÜaöå./Ã+Äß£©ãCzå©_Övðuo¢©Ü>/+<bðzð£åÖª~rößbîÄöOäAä|Ã~Bä_ãzr¢öUýUÐÐ*î<.hbü_ CovðÐ~¢.@î+ ubBA/.üUÃßãOýªZ,ªÐ@ÐbÖÖ,~,:A ã< ')
INSERT [#Testers] ([Id], [Name]) VALUES (7, N'Cä+,Ã¢ýz¢Z böhzußÜ,¢Z<ZÄ/böððã >ß~<.ArAîÐÐC<b>/ ÄBAÃöÐB£|£ö¢©b+ßöãv*£årªÖ<å_*ß©öª¢rãß¢_b<ãä+v,u©+ @uuBÃãå.ö:ã|ZÖýaAbä_üzZOåßAß**vuOaZ/Ä@ö+ö.ÃbUð|B,U+Oßß+rðbuîßbbªr¢+Z:_Z>C>ª_¢uööUð/Büb.+£B@©zb|ä*Bå¢|Z£')
INSERT [#Testers] ([Id], [Name]) VALUES (8, N'åbzuß|ÄBv£å¢ö¢hra¢*A¢ObðÐO_* ,üZ~hBýCoß@ß|A,ÃU.:Ð/|£Zß*Ãa+<£A+ÜåÃ~bß_hbãßãubo©U>ÜBªåß ýÖß,ýÖ©O>/ßoÃBO~îª*Äß:ªZ_ hýäöÜåb+ªZAoh O£ ÜUaýhzOrý_C>_o>ä£¢Ãz|£îB >î|Ð:A_OCßb_*ý©©Ðß*öåbC@Z>Ä< Üh*î££.ª:@@~z/*: Z¢A¢ßä|o::ÜA:üzbä ÃÃ¢Ã vbZvß*|¢Übürb/ÃüvÜÐrr¢|a_r@:C©ÖÃ~zÜ|@~Ðßåãaü<ÜU¢o*.ðå>h£>ZÐ¢ãB@C vªÜ©C:©Aöz+ªª@ÜüîZÖUãßBohbuäCüß î£ro:öbîÐåöOÜAîÖö£ößÖä>:@ª_bußðü@bÄü ¢>öUã>öäð,BO:î:aý|AoÐ£ä,aÃðüü©Ð ð/ oý|ð+_ªªu:äÖð:ü@<Ãßªý£|Uo~ýîU£ÜUA|zA/¢©ªCA¢öhðý@aoU¢hã*>/zÜÜ|Ä*|©¢Ð*hðbäAO<öAªBu~<åB.hCOuß~häåb.Ãbv@r£+')
INSERT [#Testers] ([Id], [Name]) VALUES (9, N',ªö+.<ßu_ObßUo_>_C  öýhhbUü |AðBßaA@Oßß Ü')
INSERT [#Testers] ([Id], [Name]) VALUES (10, N'Ãý~:îÐA©_bÐh,<ßzo><£@UýauCa ÄZý|AÃ/o:ßbª_*,bbå¢b+>©ßZUÜ/ý~uÜÐÃöUbOBÄA£ã+/åCoäðð*<üöu¢ovÜîýoå|Zäã.oaîÜðaÄ£z,.hª~b£åZÜÐÐªåîzßA>üßoAvÖÄða||öU| Üä£Z.buÃýBBÃ_Ca:CürÃ')
/****** Object:  Table [#WideTreeTable]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#WideTreeTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeFlag] [nchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[B_Int] [int] NULL,
	[C_Int] [int] NULL,
	[D_Int] [int] NULL,
	[E_Int] [int] NULL,
	[F_Int] [int] NULL,
	[G_Int] [int] NULL,
	[H_Int] [int] NULL,
	[I_Int] [int] NULL,
	[J_Int] [int] NULL,
	[K_Int] [int] NULL,
	[L_Int] [int] NULL,
	[M_Int] [int] NULL,
	[N_Int] [int] NULL,
 CONSTRAINT [PK_WideTreeTable__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#WideTreeTable] ON
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (1, N'A', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (2, N'B', 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (3, N'C', NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (4, N'D', NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (5, N'E', NULL, NULL, NULL, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (6, N'F', NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (7, N'G', NULL, NULL, NULL, NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (8, N'H', NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (9, N'I', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (10, N'J', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 10, NULL, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (11, N'K', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 11, NULL, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (12, N'L', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12, NULL, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (13, N'M', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13, NULL)
INSERT [#WideTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (14, N'N', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14)
SET IDENTITY_INSERT [#WideTreeTable] OFF
/****** Object:  Table [#Vehicles]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Vehicles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Make] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Model] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Year] [int] NOT NULL,
	[Type] [nchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Vehicles__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Vehicles] ON
INSERT [#Vehicles] ([Id], [Make], [Model], [Year], [Type]) VALUES (2, N'Oldsmobile', N'Alero', 2000, N'C')
INSERT [#Vehicles] ([Id], [Make], [Model], [Year], [Type]) VALUES (3, N'BMW', N'330i', 2005, N'C')
INSERT [#Vehicles] ([Id], [Make], [Model], [Year], [Type]) VALUES (4, N'Toyota', N'CrossOver', 1999, N'S')
INSERT [#Vehicles] ([Id], [Make], [Model], [Year], [Type]) VALUES (5, N'Ford', N'F150', 1950, N'T')
INSERT [#Vehicles] ([Id], [Make], [Model], [Year], [Type]) VALUES (6, N'Catapiller', N'Roadmaker', 1974, N'V')
SET IDENTITY_INSERT [#Vehicles] OFF
/****** Object:  Table [#Artists3]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Artists3](
	[Id] [int] NOT NULL,
	[ArtistName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Artists3__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Artists3] ([Id], [ArtistName]) VALUES (1, N'Dave Matthews')
INSERT [#Artists3] ([Id], [ArtistName]) VALUES (2, N'Live')
INSERT [#Artists3] ([Id], [ArtistName]) VALUES (3, N'Green Day')
INSERT [#Artists3] ([Id], [ArtistName]) VALUES (4, N'Metallica')
/****** Object:  Table [#Artists2]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Artists2](
	[Id] [int] NOT NULL,
	[ArtistName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Artists2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Artists2] ([Id], [ArtistName]) VALUES (1, N'Dave Matthews')
INSERT [#Artists2] ([Id], [ArtistName]) VALUES (2, N'Live')
INSERT [#Artists2] ([Id], [ArtistName]) VALUES (3, N'Green Day')
INSERT [#Artists2] ([Id], [ArtistName]) VALUES (4, N'Metallica')
/****** Object:  Table [#Artists]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Artists](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArtistName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Artists_1__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Artists] ON
INSERT [#Artists] ([Id], [ArtistName]) VALUES (1, N'Dave Matthews')
INSERT [#Artists] ([Id], [ArtistName]) VALUES (2, N'Live')
INSERT [#Artists] ([Id], [ArtistName]) VALUES (3, N'Green Day')
INSERT [#Artists] ([Id], [ArtistName]) VALUES (4, N'Metallica')
SET IDENTITY_INSERT [#Artists] OFF
/****** Object:  Table [#AllTypesComplex]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#AllTypesComplex](
	[c1_int] [int] NOT NULL,
	[c2_int] [int] NOT NULL,
	[c3_smallint] [smallint] NOT NULL,
	[c4_tinyint] [tinyint] NOT NULL,
	[c5_bit] [bit] NOT NULL,
	[c6_datetime] [datetime] NOT NULL,
	[c7_smalldatetime] [smalldatetime] NOT NULL,
	[c8_decimal(28,4)] [decimal](28, 4) NOT NULL,
	[c9_numeric(28,4)] [numeric](28, 4) NOT NULL,
	[c10_real] [real] NOT NULL,
	[c11_float] [float] NOT NULL,
	[c12_money] [money] NOT NULL,
	[c13_smallmoney] [smallmoney] NOT NULL,
	[c14_varchar(512)] [varchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c15_char(512)] [char](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c16_text] [text] COLLATE Latin1_General_CI_AS NOT NULL,
	[c17_binary(512)] [binary](512) NOT NULL,
	[c18_varbinary(512)] [varbinary](512) NOT NULL,
	[c19_image] [image] NOT NULL,
	[c20_nvarchar(512)] [nvarchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c21_nchar(512)] [nchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c22_ntext] [ntext] COLLATE Latin1_General_CI_AS NOT NULL,
	[c23_uniqueidentifier] [uniqueidentifier] NOT NULL,
	[c24_bigint] [bigint] NOT NULL,
 CONSTRAINT [PK__AllTypesComplex__78B3EFCA__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[c1_int] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (1, 55996664, 1, 1, 0, CAST(0x000EBB4B00B82504 AS DateTime), CAST(0x531002CA AS SmallDateTime), CAST(0.5180 AS Decimal(28, 4)), CAST(0.0696 AS Numeric(28, 4)), 1, 0, 0.9408, 0.7218, N'ÖöAh/+o*Öüå/ð~r_h¢Ðß@bz£ß¢åZOB@v.<|O.Z©*vãÖÃrOÖ|ö,ßåý:îÃ£,*ß>åo£:ªa/Aahäö,_Ã ª/ªäz*ßz¢Zü|å_aðää+©CU~o _ðUöð~îu*AöÐöO_+ ß båo:CðB_Oar+ä/_Ü._+ÄÃb*@@bOî©ãzÜ.UC/+:haßbÐro£BZ..hßüB£ý_¢öbÐOä Ä/öª B,ö.*å¢ h+äýå>vÄ>ßzªCîÖ@bZ£r,>ÐÃ<üz', N',¢uA£,vbbã©ª¢r¢¢auO£/*bu/ö¢¢O>ðrU@COB~:@</a/ü~ÖzBb<Öh*Ðb_ýªÄÜ¢î~î<*BB/_/Ö©Oý ¢UãO _CAObîßb                                                                                                                                                                                                                                                                                                                                                                                                                                      ', N'üÖÐã_/_>U£r£@Aý:OÐÄðÐÖ@:BÜ_Ð.:zAß/:ýð*:îBÖ~@CåªýOªÄb', 0xDCABDFADCFB9A8EB4C32A122FCB2BE23A7ECD5FB7C0EAC7BD82A4E198C4D0F3E1DE22408A6646E62ED44FEDE1AE43047618C70ADBD0C5FCDFF63D4A56B8F5FFE2363041A8EB67C427E720E3810CD61F3FA06B800DD618011825609438FC9F75DE500BBB7E015B9AB0F2D2F4BFD27BD3101E1AB64D75E20C1E2F39837AD38B3B57BB1530F11AA12E44D0DF0B03235135A99D742AE1D928686DFFAD4ED7C82FDD7797ACF4CD3AD1F56CEC617CA6E6F8FADDF3D9DD5F60597E92ACD2F17D62568CA0F1936B765C686ECB7EBDF577ABC176F6B270D3FE428E074E3E622067D0880A85E4CBA5539D74719B6B11E0B25406E6EB5BFABAF1829E6E638C1D47EA4806FAE6A36F82D01C23694DFF388330AD54FA277B450BD5DD1903566D31C7702C87436426FA0EF47416CF38917CF24A0B3DC63723107F02FB15199A6C3C72DA5BF91167E52F09A34605B36475C3345016B94B3F7B904D1C18406901D074883793E4ECA86B9A64EC2412E12D6D0A8D37D0C9700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x1436ECBAC054FA4B607D2D17D8BC090E69BE48C445F0B58AF2FAF1DC3312116F0D536507620FC664AC0261C400FAFD139FC4144D4E76EA238161C7C5631D184E6CBE0A845701AC59F8E418428FF089F7FDDE3D6EFEC32096694286004801018EEA135842F079BC9D17BA546A5DA79490D7CFC884E0A950A89D741A147AAB11F7E28A587B98470783229F57436FB539C2ADF62978387CAD40C3B0A57B5FA7236BA95D88D24F18F790C8446241520C38E73A5767838299BE66DD18B8E8A18B237B7BEB9A122D667395FE99B0F4196CEDB677BD4107409FE9109E22F5365F0FEE3C21E7C5667226B9D4E4D184E8E8CA4C712A0270E34A1C3BDC028A730B703C35EFAB74D232, 0x4450036C236E1A0027D3D5840C5D9E0AF105ACE964E7B2AE63A79C953D74F719CAA3BE1FE0DEF8CCC10163A035EC30D750A182CD85BD215C9E55087BD284C3B2DCBCB9689F7E2A130CDF27638412787777C4449CF129940C8162818AD57CFA3C3DB0C47932C738D1766FC1A596BDD78BDC91040E4027C0B228ADE63DFD9F3D8FE5EA26D44EFF44B08B73258268EE76C38AF6AD4478ECFC15EF115185E0FBD4D73231C0AC97023DDD4C5482CD023E49393622B207F2F917F9ED10E53CADBFCF909E4D50362ECA2C88B8D0F724692F13B1C7174EBD49CDE5381AC6BE871D002F8C84F22EB4EE130B6CF62A4E2101F28FBDFD337496701147D168171C670BA56AB8A06B9F77015FC3B40CE2C71F7FC33727360089E98B991B73AB5C4BD6F6A36051BF9581965318BC9E70BF8A8E4A2852A5DFF4300A1B812C074F2CCCA413ECBD6882755FE941A1D41E44EB78CC0F8D28820AA6F6027355363E697A46264B188A8402210CD5C869633E2011C62F0860F7C10349996DF5BBF42C3F217FC75AADD32B61F4CAED8153BAA142B8E1618E063509828423017E1D8E462DDF95A6E99CE4733E9ACE13960A40B08FF82A3DD9939E38EB7644D58AD1AD436683C0A2BD13B8A6D36E22F33822727844727286F3506C99415DBB72570EE86DED8AAD124A9EF37B4084696B70AAFFC903CC91175E2750151626616058454EC585B506AE75602DA67FF7456EE60129975DF26E8B7DD10EC63A97FB233D9223FA36A4E5BAA35154A39EEB9867E1E12E15FDCA0266D6C348B4A74073DD640968ECD44A38421C6EF30C9DCBEC9138A7E45868E6267390BC24725C301362DD763E9482AC9396A8ECF98ED6F5C80BD7B67B005B1A747EE3F55B1726C5E5EB5B902E278C570F8C144EAF7BED989C9AF4189BA6B51307C301C0B18C507B6FE3070D3345C7A17F7FA6EFEB0D6AF8914A58C0230D2A3F3548896B25CBA5823C98DECC74DF6556D353991CFFC627F958082F66338F5862B9C7D8EABC5B7BB3D924B2532F9E84B1E501ECCF730D406F2090131C9C3E4EAC407C13AD077A9A77D7E642DA13D4CABA847D81, N'åO Z uo©ãCA~oBr~ßÐ:obÐ@©vu_Öîå|ãîßUðOaßb©ub*o,*bA¢üð¢©Ãh©©aB©,Öö_r©+äZ.ßî,<ä£O@åå<Ðvð£uUBBv>ýÐ+Z:hßöÐüuÖuoAurß><ohrr©<Aîý/vÖaîßoar.>+|oîUýbÜ¢ýAZªÜ*ä@O¢üCîüZÄ:Ð *ära,ýC~üzö©/vZý,ýoU_ö¢ã~:,:ªärÜUÐ.Ößaåª©Öåuuö@ä¢üîÐz>Ü.u*/öª©ýCBO/Äã@r+B|~ßb,ª£ö_ß£ü<<Ööa@/~öýýÜÜhb>ßaCÖ@¢Öðü*ãÖßabã*h¢ÃÃÖãýÜßo|~aZv|hh@Äå:ãÐßü£ÃÃßððo/¢>+ßbOCýÄ:O*üb|r©å@C:ÃßüäÜOzBau¢.~£Ürzb*ªîÐ+_<aªh<Cb.büöÃ,üvýßåÄU|ö+,ßa*ßo>_ö..+Ö~', N'åOu*ªÄbß*ß+ßaAhý_£r*<ÜvU_/ýZü~>.£Zßb ~Ö©ÄrýýßÄ@ðZ/ãC,ßCBðýßOÐßßÜzüoÐrb£üå@¢Zßv/ãîba@Ð,aßoÄh*Ü~öð+u©ã|ÜÄrßåuårãz£_ýv©ßzAðö*|>öZrz|Ãî|ªªvC+ßÐäohª .<ßÜzZhbOÖåuuuU>Ãðz>ðÖböÄozý|OÖßÐUB:<.ýZ|ã+äÄbîo_UouãC.£/åboýh@aýB<üCÜAZöaöB+Ou£Uo/ A_ö¢ã/Aª ßz. +å¢|ðAÜuåZßOzZv£/ðãÄrý:hähÃÖ.ühÄ@ÜýzªbzC©BoUh:@oÜöåå:ÃC,/<Äß aßîhC£_                                                                                                                                                                                                           ', N'b+¢ÄbBßra.¢_ý<bUUß£ªåC*>ß©zUCAßüäßå@vÖ£A|u<ZOÜa ãåäãî©böaCî,Ü~büª£©O~_ý>oªb£¢b a£ý,uOübOÖö@oãvvüo>Ã_BãÐzî+©Z¢. öÜåÐßräß£~>.£aã£å*b>C+ßðßUBzßZÜ| ª~+ö_:ÜZÃAüª.BZbÄÜö|ðu|ýüð_B_ý*ßðbA/:Ã*åZr.*ãz,oßUB@ãý.,A+ýªðÃÖÄ©o©*bãªA.bvÄßrh©_,vzîãüö££,bÄÖüª:Ä©©¢ABü¢bBÖÄÃ~ßuzb@./ßãrÐÃ.:@z©C©vB_<A>zz©ãÐruöb|ã,©/raîß<î*©@BÖ/£b/<ZÖZ|uBÜöCý>Ð*åÃða©<UOÄýho¢ß|Bz_O@|ZbCBuBUüÄ>,ªz£.ß.ý|rz~Üh.~zräß/ ußªªßî:,auý>Ð|zoovå|ªzvÃzüÃ¢ÜbBîbb+@Aßa@@Orb~Z.ävob~ðäÄî@îZãrßªüCb/£OßC U*A/COCA|ã+ªý@ýO+<ýh_OzÄ|uü>ãÖÃ<bÄ |ß', N'c58b7a8e-e97a-497f-b398-16b9c55339a3', 9223372036854775807)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (2, 0, -15870, 96, 0, CAST(0x0003F9990007BB4C AS DateTime), CAST(0xC12804DA AS SmallDateTime), CAST(100000000000000000000000.0000 AS Decimal(28, 4)), CAST(0.7966 AS Numeric(28, 4)), 0, 1.79E+308, 0.0000, 0.2276, N'ßoå|Zr.+BÃ£*vððv~v*.~<ý©bðv@>.åCrb<u|/Ã.©åOao¢@hCbZ¢ärªzou<*ðîäÃß@hr/ÄÃh~öÐÃzü>:,C,/hvUa:ob*ü <ÖA<r', N'zÄ.+oAî:îaOÐAöß@/|_vhÖZ£*Ov<ü©ßÜäüB.ýbýu __oä¢ð+©Uªð©+ _/ÜAüÜ/>UO.åbä,@+~zðªuCÜÐUåßÄbåB<¢_ýuba:îbzü:Öå>vÄu*+/Ð zýZÐå¢ÜoZz,/ .z |ãA|îh ý+,_ýã_ÐbîrÃuÖv|vvhÃh*/Oä.îÜ@,o.o©z+>BÜ Ö|*@hÐB|zZÖZÜ:O¢ðuÃ~ðÖu£ÐÜ@_/åbO<ßð|aðACz*<u£å£Ã>©:Z,r,a*ÃÜ*r_C.u~ßü_ª¢ýO                                                                                                                                                                                                                                                                         ', N'~ZýaähãbÖ/ÜÜZ.ðuzhÃðÜubabb©håb:CãðßÄ*Ãðu+<*ý/ß@_¢,zä<£>*Uüöß:ÃOÄü£ö+ªräªu:_zäb|v_C@>Öª¢O/:<+ÖCÜ©z£AÜãoö/@Ã£ABO>O.©v O©ö+AÜöhÄ<äBÃåýrAÜ/,OBü', 0x6AC5787019371F267E1058D3BCFEE7DBB9BD60BA985FF45CC90C8F606E3BF321574010411EF97E08D0A324944CADF407C96B36F4C66BCE0BD01CA70DA8BEB8431D367B7CEEA5BCC03F57EAF53B5F0F1B1788960304FE5AEC7205700251716011EBE0761531781823AE75B56F81D9D0710C8BF51F27B43E1EDC8F0AE934B96FCEF7D80A5AE8FAEA9F6F72558F49EB6B9835FFF6C57B40E936A25839A26D2FEFF406E677A8CE768811A155AFB55FAFD5F02371B434C3A952B6EED3B790C14731408360886273C2C9BE77E423269034D7F283487FFC7BBA315D94C1BAFBBEF7505A236F31FF27727D60A7F5BDDE852C26B9FBC40D78C835C4C82E2BCDB207CE007F93C9022103D8CB10CB1E5586FD52D7689A0100F9EAD891EEAA68D0FBA4CA59D9F43D4BBC2A05B623F8FCADAADF3ECBDC53651706CE0795BB3713769A08CF6C00C47C91C0071B3E954BDD00ED2AE70E6B9DF3A83EB3520BEDA78F84168E1B1A298FADE563AD0BD5382682BBF006A7904F040AFC14272D4D7270A418779627062FC2D75E7D01ABBE921E6F1830971AE7FFFE3412831E0601616073F208F25DA1FE7B8B2E2E49134E594C6AF6B7151CDA56BD187961158210BCD6FC1D8EB8B6A0C1CD19B50BEBB8B6EBE6B837B40F494C43989C4BD9E340E9F97B72584DF5BFA196CA34E9FC67CF6A05628D00000000000000000000000000000000000000000000, 0x4A5CAC8CAFD9F51C6AC6DD8D30BB90795F69F52717A1E9DB5F50DCC931467755C58CAE1EC5CC9D1BC9852B03575CCCAB3EF1A5F8975344A872D12C5FFD2CEA234F87C8A30D71B392CCDA5D9276E6840384318A3FA07EBD714705ABF4A03D1E9D9A07B3CF93289DCC3D122CBDF5B2328C4C28DBCBA1AA83D1C9565B08C5BFF28EBBC0F86E32B4705B94BE028E51007B95D21FA8776253FBEF84E2053CCD95067E1F9E0235B9C35997B88037E3A8F480C9DBC535A6167A2C68C509EA50AF338DFEB7836EB3FD46DC0EC510BFE11B18468F613C61085F6067A5719A6FB0D909E884F2382B2486CCCB94E926E006BB4A4CACC25AEE73D2859F571CC94163D62B35BC273BEFE3EC21089570CC1F388060E9274DC2EE7E1A75B26D99D408BD8A67BDBDF123DE805B69BF, 0x5902056566D7BBDF1A15133DB6A1B288814A18194B0A080F61475C8269CCE4034D2DC172AD0732FD4DDD7051A9803B812CF7D91FA787064EF9F5031F7A39754E3110EF88E03FDB79171BCB6D9AB665E10BDA5572F2C55BC6277378B7031282136821209890B2A1EC1B5F078BBB98B443902214A0E3035B6CB529616CC624750599B8057896CC515938685692BED00A8EBE3527E1FEE13065C5B4E367503426F7E901F203923BACC3D0BA687AACC9ADE51ED2AE90FC9307B888A0B2E2E8E90803425FA8BCDD07FB838863114379B5B8FC06FE497D8695FA55FA3A7BF288CADAE7B264374D20F10716D72A079982C33ED2E7B6DBEAA08B7CAD6452C2A38C09FABE882991157D6BDC14F733A65C8D61D336FBA80AD2EFF04727E78AE8CD2F6389CD304911593229557CD9C6F26DE42020037F8BA8E4FE27CF97E26A2DF3820EE594A9D82B43507904D2528EF828FEDA42C769E54DA4BBA5CF087A7976C7FC62F9CBE15EC30BF4DCB20586D691702EA699082765899CC041D9030923C8D6033DD5F0942D479F1637D28D32471DE4135ED9D4E06D9F96D95D0A9F85BF01D113DC024900A1DB015FF9F2F093FC697823F4F0AB49F4F6969D3F163722A71A563501795645D4E47283D875E67BAF02DDF3927B90CF56881FE56CACC08AA3DEB7DE9BBB928212334F0F7D27707F101BEE3C3944902C8986DE785A71D92724E7DFF9BD07DFF3705AE05E2C81522E179FD551BE83515F4E8B5515DC1928A15927EB964877C1083BFACBDE6431FF0834B061BA9B8869A09580AA32B2C91757313E050496BBE6A0DDCD15C3B772FB1C8B4CE02CB86D06D1BD4A00C8CC8E5572E374AC252CFB7279C9C0DDCE72A6E823848EA765E9EB9FC43833474BBB1ED44E77A3E043C5D5F090DA6762F790375E1761DD433EE7F088F7D4E8EDF9B326BEABEC8E57CFC6A55DAE9D178B8188A83300D430E9DDDB9567871AE86EDCE8F6DCB3A9B70A02, N'aoÄ>oröbaO¢ã>ðB CÜÄîU|¢ðzað£O¢ü£©Äa¢ü~ßUr.@©bÄ:+<uBÃðãÖ:@ßrhZzä+rüoßî<*_ü:ÄßAüã~|_oö@ZÐßö|BO+©üå_h:C_£<ýå|Ä<hð.|öª~ÐoðäðU*ã*,Ooz<ãZî>Ðbîbobö|vÃah_uÜbuÐ¢Z_årßCªÃ..ýZ£@bu|ÃÃýãî*ü£ßräö+uUC©¢üäãbð|+_ov.+¢<ýr,_ÐÜ+Oî|ýðÖÖAOÐOüåB+ÐªÖ.:+rÄU/~+öOãÃ+îUÜövBaß¢>ü@u,Ð+¢<ßßzzýýr _ +ßä*Aßb¢z.:>ÐªãäuOaAãa ÃAîb~vßÖö_©/ã¢¢</ BbuAZbÄ<CÖA >>+<OzhÐ<+@£>übOaUªÃ~ÄÐöB©Öß*ChAÐ¢AOb*:z*ÃãÐ_~@za_:åo ÖßÖ.+@|z,+U<îåðåB£Üz*îäa.@h/ßß£Uå,åU.©ýöÐAZ<<b©¢+C/ßCäýäðÖýä+C>|Äý~@aÃ:.©ã|zª,@*ä>bÃ,ãhA,Az©©C|¢@ÐÜ<:o¢Ã£aoZZ©U_u¢aäbßOää|Cý+ßÖ_vößä¢båÄå', N'+©äÐÐÜÜ:bCÄ|Ä|î¢îA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              ', N'äv¢a*å,îoÐzýÃÜßraß,u~ Ã.|+bzÄ/ý©UboBbã,ã+äZzßÄåöÖÄªB£.Ä@|r:ýßbrÐÖ¢Ð¢ªÄhO/_ãÐb,ªã>C|aßåaß*¢_ä+.AZ©__OýU*b|/£C~ý<Oª¢<Ã@Oðä¢îZüovßA>_b>ã.äUÖ@/,uîU*o*vÖð Ü>¢Ã|bü,åz', N'99999999-9999-9999-9999-999999999999', -3311196357)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (3, -384189910, 0, 0, 1, CAST(0x0026421700125688 AS DateTime), CAST(0x901A0001 AS SmallDateTime), CAST(0.0000 AS Decimal(28, 4)), CAST(0.0170 AS Numeric(28, 4)), 0, 1, 0.8639, 0.8536, N'+£hÖªZr.oabýÃ,ÜÖBbã,b~bZ~_.v+CO ýü~*ªÜA.:@£ßöb.ßýßÖCa,uÜ~äObåöAAãÖýÐß.h a.ö@öv*>C¢v|+bz~ÐßÜÐªü_@<ªöObÃý~©/B~/Ããz@©öU©:©Ü/,ýa¢,Äbå/b zo//£Ãî', N'ÐoAü<uðAzãaöOÐ©ÃýövÄbÄa£>a*U@ªý|ÖßÃUöAoOßA~ªA,h.ÖÐ©Ð£rðAh+::åÃ>Ãã:O+b.ÖöÜBðAC.¢Cäð<ÜöUð_å©C~~ÃZB@åÖ~rÖ+*ýÄö|vîîÄÖ*~örÐÖ~@¢>+ZO+bßß>U©AîC|hßuz*výÖ,ßuB*åÜCob~Uýbß*åÜ:,©bÄ/<,bö_ZO*@¢å*hý/ðÐC>ý ä|Büä/~ßªB*Bß¢üßã~ååuÐ+o:uå @©__ÐCªßýü©ä~@î__U**<Cå@¢CChOö>£_vzð_ß+©AhãåðazÃßãªrå_ä,ÜAð¢ý<¢b:A  rýýÐå©z Zaªªß.z* ZÖý,ðo@äÐ©|>ß.îÃü<ããä uu_CÜ. ß~oZÄov*ðZß<åhýÖß<~Äðßö:>:ü_Öãß+B_ðB,ubU¢*bOa+ýOU:Ä¢OÄ,ý¢ÄÜvOÐ~b<:, uå*~.*b~ÜÖ/h:ßZZCOr+hð:Ã/ßbåÜüÄ|ý©ö*Chrã¢ßråv£ß,/ZöZUaOZ.©hªZrßßbåhÜãðOýü+Ö Ä~/ßý©:<a. ý<©ö/ýª|/a               ', N',>A|>Bü åUä|ßu*ªªÖ ß£|oZOýÐa+ßzz ÜbZüCö,bB/Ð¢Bhu¢ýöÃÃ|Oß*Ð,@*ööbö_UbOb£>ßðÜ|C/©bä@,,ä¢¢Ö~uöuÐ.bBh+ßbäbb| :åÖß¢Äðbz©ß||rzð<:öh/Ü,å©.Ð¢~rÜ<ßb.Ðäüü>ah.Öu_~O<o,ªB/å©>åCrýZ<Bå Ð>ß~b>>:CZ*¢£hã|o/ßo,,OÐUýãÖ@Ð©zäåßB+îBåC£zª äuý+.CbZÐö  b*b©Ðð+¢bÜÜ ÖOv*ðbÖB|.+Öã,B¢ªAhð<Ö |au+ÄöüvB:©ªUãÄ*©rUÖ£ü ß*ãÐaa£ããýäÄ ÃÃAh~hZzOßäÜî©:ÐUa©~Ã.Aårüv/ªß¢+h+£ýoBo>Ãa£_ýüC OBªüOC£Z©oð¢åãÃ*O<¢_a<|hãoî©Ü<åUãðv>u£OaÄ+zaO@¢ä Z+> ~zÃ_:CAuý@OvÃßö|OC@Z¢ß', 0x52D83B71DE1A8924A0A53016A9D84A04B876A6B837CFF9607F418547717F4B2E861797A6957B3FDEAA990F97167DA8856132A2834827D482DFDAF19C9542B2215A018F9240A46E3C37C80E9EBF614A0299FFE53F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0xE65E4315C7BB89A24967E6EE3D15ED1544BDC3654C801835C27D90EF03955C7244921A314E2BDDB3B3957EA7B7D124351483BB287C777B66450D52492A9A9EB40B74AAABFABCC718DF0FB2B701717EF1595BDA80D933F5CC16B4EB41D9948818F6CA9B4226783B58BBA3486CC5C465D48E60EFCFBF1EDAD77EDE9445D1863E4B8799C036D53CCBE02081C436EA8907514F17B279A4B859D7EFC4C448E4F5873564FDE52B032EFE52950EAF3D5AA4ECF5D642F6BAD4C5A7CE40E946117782FC2AFA8ED1EDA3DB4E20185FE396091B7D4BD8CE71A1FECD619E5517EB1AE7ECDA12118443CBD2FE51FBF4D6A174E111C4256BFB39A9115B2C2C2D32988CF033392C84D18484D7B179D97C76AC282CECBD4452C6AEECB33101B658169D9F66C50AC19D74EF108C404DBA825F2C9AAE0400407553392A844C7953D0CE2CC114D973B76BEB8AB7FCADB71279F46A5305660753155C2A128BE03D50FB9C3FBE138BA5A9EC752FC3794944C89C27B18FE2264D69C6D8ABB7AE602B8BAE3ABC606DF8FEED4FC066903744DACC186276642FAD6DD54FD5F83BE0EEFA8C4AA4153F8DE35AA329C34021BFBD3282FEDE9E4BEAC65B00CCE6109700C20C60C6DA46922E9A3F3407D23B6B6D4C1591, 0x3C014FAEA183481472A6D3C3A366B7DF5BE5779F53639F6A44178BF09380D17FF48A860A101738C24A19289961442FD4B48DF02F1A0322D962E56A8AF75881F2D453CF19E0ACCE43ADB455393B050800E8B73B058FA164F067ADA82BADAECBF19835C40D7515F407BEAD8265ADE89D5DDD6AA1401C7B6332EFDEB13304A396FEE963A10540FA8BD2C23447E21FFE427F0F4B4E420C8AD51CD2911E9664C1031A16837C4A46981CE2DFCF7DE8348013F09B3124B85553F25E8E8532AEDCF46D01302CCB9B81F839762E6B633C0CECE7517E2DFA329D4AC5C3911DBBC0666D05EA617CB808B5786EA3F8AD8AE9EFB551A19EE030AFF93A63E5670709633D7810C16578DA9034DF9C05BE573AEFC0DBD3DBCC1A22853C56F02618D2A1AE6F0A66809DC88377DAC16CBE442BF0A20074248B342D9723558A840F88B477623EED96B3F1B952F7575C010AAD644111664E26A4AD157A4172422B124474EF264A0A8056943CB046212C34D72C8D75F14972A9398DD2B428735540AE8E3FB3E26DDABFA2E3EA7C299DDF8281E1A214E5CD39C9D1CBB598785657E23FFB02B8E279516A51CED30D5EF57F9CAE4F3347D1405BE15475E117B742474C8C083D2D9A27A0079037C216FA2C6BDC4B19E928230F11FFA021636C9839B20F87868055B8520187AF2BB7997D45E7280A04CB9E581E297DF73E8D79C3336F5685B38F73AC1523F9176279F75777815A393B62936E13B7685D035E7AC487C37BBC6114CAB5737CF4E0D187F5A531257FBC324088D0DBE771FB2716A8B7A9358F9EFD3AF663B7A5C5BF6EB29C8CC2C32BDEDEBD91473AABE079A2CE8E5DD33D379CF756A8F72E1C058D7AC962294BE53963EAC9D8ECA3476451597CC0E81668EB307B00946B1A34AFC1B804CD7148B72EBD952A7209644AF8AFF2569DB25B1D7612DE31AD4B50CD49B61A16349E7AF209A24E0352ECF5E1CB3007E578C496FD669A8EBB92160AE26CC8E1BCEF4E165A35FF4DFB6358CA891CFC657C436B5485201DB09910BE5FF2976B978A0B38F27D1AADAF79779B990ED0049E96B08D9E00735C9CD19D7D49769F5A63F7C0407EE30AE86296BBDB0E8483A2C80B20C9410ACD9617AF9C20DF9D6339D91C37A14D9086BA8CC82A0844974D174EFBB43E56D619582A2A29CE47E908C5338202F14CE1553884653D78E0C4517F6185D2E4E825E30203DF772366B773A2C064D5C2955CCA6CA67BB9F099A07765D8A96DC11724DC0D9FFFEE6D6A31D1CDF29BB87BEF8FE905BA653D6D6BF2696267114ABD4AB28A9B88C722966D5D0174A9ECFEFDE7FC7548C06806B1E66FA1AD958C0559E7148340101DF3286D53975D64C9C37FF36B7B6B9B01FCE1ABD9B519257500E157A88D9468BEEDCA084DDB6CD0A84D1DAB22B92A109ABD, N'<|/r>:ã.ÖbCBAÃuvC+£b>.:ãÄÖaÄr>äÜßC@aAaZ~ðb|îBuu|vÜ£Zå UbBª<Ö/CrUr*ZðOuUBbbh:ä_Ð<@::/ä~å:ä:î.@_hßäzÃÖðz+b__rãrßO|äýuý©Ü,¢:ãbÄzß~@î ~|ãý|ub,åz+Avbv*_@zohBîbZBU£ÐÄöÃðär¢Äå/aîvåbU/|ü/üvbrÖO>£ÜÃ:ü/©O+ÃC,å££©bzUårßãä@ZrÖ:å<oÃÃüðOýA<ªÄÄ_zOBß ªýå@zZãöãöÃuîÐ<uöÄO©Ä>BZÖv>CZ_*O./ãî~£+,hý b*vÃýU *:ðCªCîªÖAßÐ©Z~>ß<å~£z Ü_ÐoAö>~@äöråvZßÐaüå ª. b:ã:uC©ÃACh~åöä©v:ZäÄBÄU@£h|©/aoC,C~Cb*.rvoÖCÄðßoz¢ä', N'¢öÖ~ð|üBAªCü:>ãv|uCå£:ööhßÄ*.*ßhã|Üoã@CÜ/Cu_£hv/öåÖ,Ö:îOaß:Cä¢Ã©ßühåÄ©/AB /@u,zý|v,@ª.,,£ªoAbaðß/|Ð©a+Ðv:Äz.a,ö¢>:|~uob,~ÐuB@.üB.¢B£££ýO_hbåCü+v£@@ª:*~v<*bvo¢ÐAÖra,Zzüä:+ABBrZ¢+ö:ýå*£bAãoz|ÐAð,ACîöýý*_©hbz>oZb:îrOÖo.|ö*:h©ZÐ*CÃbb,©+üB/ur|Ö@©åUCßãÖ£åÄ*ý¢@uå+,ÐrßBr©Ü@ubzðrýrãÜU.Oð>+br+b                                                                                                                                                                                                                                   ', N':å|äßã_Ä~©~*zª© ö>*ÜßÐü:O<ußÖåãªBÖÜrÐb.o©Cr~ßãªîÄÄUåÃîüÄîB©zªÄüÃãüU|ã:ªåBrCÃ_,¢*å CA£üðrßbz¢,B_öh>Öã~bbUC+Ü<ÜZo@< /v~öbäbbÖÖoü<ý£O_b¢ÜÖãvåðZO/.@ðÜob©Z*r/åß@ZzbhåÜãªÐBåÃ|ªýÃaÜÃª£Uã+ß. +ß*ªÖýU_üßÜ.,ªßÜßî<@bZ£O©rråb:Ã :::Ü,>hÃ bAåU+Zýß<A<*îvBooö_ß*>@C:ööhA+ ÖÜOaUÜözîrb|/uý_r+¢U_~îü|@ÜÜ.*äßBÄÐ|ÄUã,u¢£~ZðBå<,ÖÜ,hÜOäu:Ðýoå|*v<Ð>.ãö_Uaðö Ã¢/åzaªB,üð_Ä>ßB.bÖBUa:*,b|ßuoCäu__ßzBäåüvå@ßöÄ o:ãöoãb', N'99999999-9999-9999-9999-999999999999', -373556134)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (4, -1, -18327, 20, 1, CAST(0x0000901A0000012C AS DateTime), CAST(0xB2C50093 AS SmallDateTime), CAST(0.8687 AS Decimal(28, 4)), CAST(0.9782 AS Numeric(28, 4)), 1, 0, 0.2299, 0.7795, N'Ã,:ä¢Uªª/o,©©Ä¢_+o©@ÖzÄB_U>Aåuhbå©,ðÄC*@Ä~/ü~:väãßä<Cß.,©*z,Ö|ÐrA ÜazÃßvßhßoäÖ@Bý¢rb*', N'bUav*ßüÜhäuöU©v_hýª©üaU@ðßr.h*Ãä+*ÄÖz/::ã+©z¢ßv/ÄbbbÖýabý< _ª|£ßO+|îö>UÄ_hß~*öåC_ÜÃbZ¢B+*£ÖßBðäZÐåhvU+<uß*@ªO.Äb~rýßå~>_ZO|ZÖ/¢ªZ@ä<ß¢åoZ<uöC<rðöÃuåðA.h+~übßO:,Ðî £|UUö ß@.+_/ßý¢*brÐÃUCÄBu©ã*Caßurh|+Ã:v|B>ýÜãoObrã,OýuB@ãoaZ<<.,_ U£.U©~öO~ÐCövb<ÜoZª¢uübA@_:üb¢Bªu>î@£öoßUß£*,îîðßÃ hÜîBv~+ÄhhüBö£@ÄÖoöhrCß,. Ä£¢¢@o,zöß .£övuAÃZÐÃÄåroÖä|CAý@©ðåÐB>ªßÃ£ÄÄ+ @aåýbCCÃ£ýÖ/<ÜZãrîßßÐÄb@C_B<AbýOßßUã©Cüå,ýZåü©aã@ä/ßå|ªßü*ä@A£©~a~ª©@:oböö.Ö/o|:@vöoªrzUu:å v/ãÖzÃå,b*zbÄ+äðoã*ßß,aÜÃ>ýßZ*:b|ßß>B£/ÖZo,Oöî/u +B+BB              ', N'/bbßÃbCÄåU>î|*ß>,ª@¢aßßßbÖ<îUbý<vöbA~¢brbÃZÄ,_©ý|ªü<~ðäªUZÐðªu:ßUa¢ªZÜðÜ|z|z_C,@B> £C>î*b<ÜZåCªãU<:+ðbÃÜ©rîv*|bhîuãCåßÄÖCv_ßa£å:ÄÐbb£a~Aß_ýîª¢a@/¢,©v<ö:ÜÄv£@îÜAz:|ª>ý<å¢ß *ßßh<va++UuOußªzöÖö@åÜöªob,a:h@b:o~£ü|Ü,>_,ÜBv ßî©_CÃZ/~äOý.åýðuÐOÄbrö +ý_a*+o@åÐ|CÜª,*OÐaßÜ|*>>ªObð_Zªö ohhvbuÃ©CÃzåo¢ ß_ Oo_ß¢b>ÖOCª /öo//Uuî£ßBC:©r¢:_ÄBaü/ßß*>BÜ A~bÜU*ÐÄOB~¢©+b/*üãåZ¢,> <@*£bÖhhzÜ@A©/ãåÖ+¢>+ZbÖ*ý_Uåb,O|:Öß+bC :>*ª@,©_aý_vÖýöO<ªzbuåUv, :u©a@ÃUZAåö/uö ý', 0x2BC6A458B7BBC52CBFFBD6613AB9B1FA2074449FFA3654DAE9F69C195D463450A4AE1EBDC4BEB7672EABE8063224E150CD588E5657DE1AF472C96EC11FACCF79C685BC8B9AF33562BCDD714E4D4DA8C5144B4B05B8DDF9C6932A4BFA50F64782DC6F3FAC6755B6D9222B9A796CCBA62421A9ADD360CAC1E88CF6F770A83A11C695EE72DE0DFC5DBE9472E28C437F59C88326D9A749732112AE571F705D4630E81AD3E37E59C816254BEE3FEDE734A40D9D2FEC81606A7D22CD5F30B6DCED604E41FA726F9CDB6EBB10B35B5A34332A79B212D32D59AE87AF56C1B1F99860495E8DF0ABEDC23470C1C03171AD0E22ED9934B603CA1932F59372C219DA2A7522B06AFDCD44873DB77EA1126C273DDAA034D417C4E25C7E445AFB187AFF1B95974B0FC1421054A911F7FF852807C5CD569821E40612B04053EBB873486FC3B7620B7A21187C9790DF2A06CDE2D7563D942DC2E2DE368E849ACE6D5657E160A13447221C8CA4B4DA009105DB0BE35D7B957BDE1BACE1D92C7CF5864A0E4038FECA65999BB539B1EF78E8E14C31B279B670F2C0DC0245F4526EF5A0961D6ACBC0590611A64AC2C65CC9EEA2C4008CFBCA7CCFD91C7F5BFB8570462CC0991AD20E1C20F32AA1D36CAD2C641214A33052E2F900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x00, 0x90AF40FD0B8B36049605C3AF1C93A7DF99D128A9632CF01B49E7E23FF34AC6C3FF847D6DE0F51E169C671726A063F70F89734828AD71E964BF24B424A8F6104C3F00AF98163AFEA3B2110CFB14C97AE6EFD3B67F019D158E9A19ADBC41FA6DA657CBE6A0B35EF94E75A4759B65EE4FF5A9CE34D53F904AA2EB21FEFD8C4262B8A465A449E3DA3290DA670A5C277929ACC9B81272C59866C68044B59260FCC23242BDF7FF490512CE9BA3FBD785ED7A71F75133D430EFF20BFFDEC99F25A030DDA5D81A6A822A63C3A543D7229D8E4093C9439A0EC8ED0252CAEBFF6A3C72A9FA731D32ADD30FEECDEE702EEC18D06E709E35D5E1062115B8D5C8A0962AF886329ADCAF805C92BFBA55CD2058BE1639F97D8ECC069DC7F4520779D80A6F58A2F6E9FB523C21ED140B751B8C3901BDF9BF8EA7199B2C7F95D2E75C78E2BAB54BFD2A2A6EC3C3FCD78F7AC090823ABB514A80187BC73E06C969667F55C4753151C0D08F0ED64E35FB91CF89216BEE12982B0B2D62A13983BE2360055D44A71319486FD0B2AAAD7AFC4A1DEAF8B5E55E7A53E729B848CFAF874DD1EA7648D8B7EABC6FD8FB1E60ED7E913017844F8A093E018A095C7E5AD179BF4865D9C203B37165FF3C450C625BF2F9AA2617B82B38C2C8AC75BECE5A6B629D5F3CD6FB14BC8E7CA51E4F4EC3319F2E1762D433BAC19410A210168EF5A20A94D9A7AD2685F6E5ED989A120CE873047AD68FB7F7972EE39EA7C55803A600870214BB741C8378A8207DE69B20AE88272E0C9BB865EB12FCABC40F7FBF9DE24797120FF72DFEE8F65DE7977AB193C3EF1F9E2AF60A48211701BEC95874F9E8829A9E030D9E90F92F24A55A7E53984A170120602173EC3D4FF1F8F66E358F8264FE15E01390B5508EA5FAD265EEA2C36974B2575399C70EAE61B31BA6998AFA9422F6803EB837864BCFD17B7919D8614E348DFA82382ECB360631B2DD99AAF51AFF3E42F72285B13C2C0B84C84B94948852CD710C90785BF42B4A378685A9B124DC617ADB96692BAEDBBD562CAA02EA729C298ACC38E88A86E24DCCD84D2AB7825A5FEBEA19B9F2E8A1C330D2212E4A285FC5E95C1EE5EBF3D7F3F9489C5360B006D3D24A160058E336A22A75B7B765B303A478, N'üßðaoãbaðZrrb<ãhoB £roÄ¢b@Ä_¢B|:hãîCÐoß*_>bÐ:|vv*<vãªb', N'Czö¢C>,ßÄÖbu~Ä,aZýBÄCüo@v:roãÐãbðBý.roubBuAÄ<AÃ¢bv_¢üOÃoh/+©ZUýüîaZ<ÄýßCÖbªåö/AuÖ|B|ruCÖ.ãvÜß_Öäubß<Öuðb<.åî|ßrÄ,oðÄh/ÜÃî_bäÄA rZÖ£åª|:_£aBÜ*rOª¢|+£¢bÄßvh|uhb+/B©¢ßhzBãÃ,bh a*Ã*:uo©Ã~UAä¢<ýU,ª¢£Ã©~*<z:@üUý|<UÜääÃßuÜ,b+ÜZÃªä/a:>Ã ©CaýîÃhÐ*v:++ÃC,ÄÜ.åZ~r|O¢ã£@U@bßðCÖäÃî@¢©ÐC îzðÖAZÃbßAbð/ßZä>rBÖ,~/~+_aäîU¢äBUvzðî<ößýýîbã.å¢©h<¢+ð å*,>ZßO|.uãO_üð~Äoð.ª_@@ChUuBhuÄ ö/|@ý©ýß.>ßýU ©Ð|Ö</@Ä ßä,h~ö<vaCäª@*bÐ>ðªrbÄ_åß|                                                                                                    ', N',ÐZßuCÖ£~@.ãðußå,Ðo,<Aü@ð@ZC @ A<öåvãb+.ª.hü@Övßå£>b.Bb*ÜÖåÄß£ br++ý*|b *ÐÜbîýýOv<UÃaUª+å@<bO©ªöbbÜh+v<z~oÐ©zý:ÜÖÄ: .£böÜaCßbýoÃhãýãuäßýaðBä@öbîbÄz<uv_/ýß+*o£uªaA@<ý¢|îî: <|r/ßürÖ+©©<åZ_ã,</ß*Ã<ã<.|î/o,|Öðbåß+_,öîãö |¢r/Üo.:¢ýÜb*ü.,zzuvBÐüU©baöbä:ã_@ðÜ+:©ýýCCövß/.Cý£~¢©uã©üÜ', N'62b77423-ae37-4fdf-bfc8-646b1ab8dfd4', -628437411)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (5, 1140434383, 0, 6, 0, CAST(0x0016C41F0049D9DC AS DateTime), CAST(0x545E04BF AS SmallDateTime), CAST(0.6171 AS Decimal(28, 4)), CAST(-100000000000000000000000.0000 AS Numeric(28, 4)), 0, -1.79E+308, 0.2235, 0.2926, N'.Ãöß>Ä*, vª+>üßö,+Uu£ýåB|£*_©Ä£_ö@ªoÃ.ßvv¢¢ªar b.ðã>bå:', N'ãvä*ßÐ¢¢b/ü*£Uü~üöa+ZB* vhÖåh<bbr+äoðð:îaýöÜ+ÐÖÃäðBð:b>Öî@B©üÖ<~+îoZ,ß£b>C*Zh~ü_+bau,oªhäBv,ßßðv@+.:|<ZbvA.*Ãuªz|_zîÖvOrza.Z|UuÃOBÜÖO>üåÜöö*Ðv:£>öãZ>a@vrrh|ba>AßÄoª/ähb|.hßzß<b|Üßª.ðrO~@|ü*.ßÖ/ð_aBãr£ßÜÐbåÄö+ÄBaZuå¢å@ýAðZoÄo~++,Z:öÄ~oß£rhzå|OUv._@_z£>CßZ*¢b~<> _BvhZÃbbü©,Ãh*Bî/ÐbäãB¢üãåZßO|¢£ß_aBýCCã:: üC/©ðUÐ¢ã/£>::Zz..vî,AÐåª aî ¢r,+Ü©ä.u+_:ößrob|£ÄßÄ/O|ðð+|bªUßZrUUC©ß.îObÄ::U_Ä_Ö+r@îÖußäÐ>ãhÖz¢©~üððå                                                                                                          ', N'î@,:C:Ãßä@ãä¢Üz<AzßbðbbZ.ßß£~Üªå@îª,+Ö©,A©ýªð/aîZ<î¢bß+ä<Cbz©bUbðü*ÐoîBv£.avb/*üßî*BåýßÐ_ÖðvÄ+Äý/îÃüBvÖ,+¢ý£ÜCßAÖB<ÜÃhrðo¢r,ýã|vÃAßoÐ,ä_hüÄUahö.üå>¢åö', 0xFAF4F635C3DA1419C88997225150F519D7F747ED68760003D79285875B4C78AC240E3F9AD7C5AA446042B270000339DAFA419E5138471883F3F35D30548DBE7B11EBFE42115A41114D028D26C49002D459AB8C1AAE2674DDF5BBA7E3677AEFD383375E1405F6C9A0FF8C0435BAF2BE63F089D6A900A4CCEA76204D55B35DAAD313B9A28D31EDCF63E1EC1A21213F2203FC43000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0xA345D2DF6A749EE4A973ECA1E231220050313103BA73C71489AAC837FE49586F6BCB2FCDE7C223BD4FEBB96EFF793FF01C21B2CB12EF24307DBE56BFAB66E55F1B7D36160155198E0D73B3CEB959150F6BD81BDC978D5D7BA7FF4F296B6312E9055A53814329EFC70824BD9E3C557624A84754D24B09C88D20BB6E0106F022AA61CAC95F0693CC, 0x53D372CFBE3E52892A80240E3F9DFB92C56188BAEDC01802133D728B171D157A12EAF73555BE617C1C78DB6FFE9B212CCECC41324487FA93BB70BB81CCC7710D6AA9FB55A6C53D06FF0B9E75E5A803771B45BC4ADCE3368BEF647AE402E0AF5406CD05549FCC86088F3A3338E985AD126C4466870A263187737066424A58031F5C496F17A3AFD84FBFB3BBA1B0FEA26AB66878BE79284ADF9C7DE12F6643BE373713DB9F15956EBD6C17C7566B8FD775CDFB8BE2DF44A533FE8F7A26CEA8E87BF5846A9C22025420F9BB526260F4740CA56C616AB8DC57F26EA8A1851B48EFC32E4B60E7253B53CBA66AE824F242D30A1B6D8061477EC3192C772D7A61CE05B1CC8F3414938CE76A1996C34DF333002BD5966AADC88D7CB08847BBAD0ED96AD7643758620EBE768E861548B1BA7525F87701A5CA6458E93E13848C4CD1395C402CEBE9BCDB9D1D1620DB26B244CBF51322C2350C339F730C4E194F8A322ABD4CB90885CE8C681DDAE38EAA893136FA478E78290F034F0C8C78DC6EC24D93E4F51EDAF93B462687B9A55B28A5F5E9DA171E07FBB900AE4869FCF856AFF95B8C50A9B46B573DBE0168F16F69A82766968E0257A346972BF56F918F3F4DD0D7C04ACED7FC417E82D95F0593C89044A0429661C7ACA1B746B97DBFFBC6AE7FB49719AF1FD94F69550F7F7D43400463642BF14CA8EB082A1E92881E7E8283BC2BE051FBAEE124C6F32C0CB82D247406B51A5ACFD561D6C88FEE908D5130EBD38CC35163EE08B483072A597CDF50FDFA1E82696427B3E7CA09896CC0509675C241392B6AD21017AC65D7C8AA075AE93FF0966FCEC786ABD8DB875A659CF43270B4973AA62493270D6ABCFE4FEB3CC3AE9F2DCFB927A6FABA8D538E4BB6B21070A457F66F20431A5D6E61640B875BEC9FC3ACC56F2579490D9C7F18D105DC57B7AC8EAB9346538E5C12724BC542EF333166EE870AEDA14A2B0EE8D3A02A4957920AAD5FF9B059586F1D685CB3, N'ªvÐ/ÃîÜ<¢ä>åÐvÄo~B@Cîa+Ðö,Üîu:>o|aªÖ/ãÜªzðªZåªz£oªÄ~oßv~åý>Zhävo*  Ð©üÐÐzðßü,bã', N'hÖ¢ÐîaÖÖaä/ <</©:ð|Z~üräüvuaZrßoorýC¢/: |ü©                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     ', N'.ã:+zöhZb>o_ÖÖ¢ÜÐ:ßAvOv< Aa<uÄªzB||/Ü¢üU©äCÄã~ä©ß_ã>äbAU|übvð©b îä/rrraC>|+,b|£ÖO*©©åB/b@ äß|Äzaßßßüh£:ª_ÄazÃ¢Oa zB~ý,:ß.,©ä r_ÜU+~vÖ©ª@åbCobC*îhä:vªÐa.*a_Ð', N'00000000-0000-0000-0000-000000000000', -9223372036854775808)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (6, -746807457, -28201, 7, 0, CAST(0x0024B0120006813C AS DateTime), CAST(0x674904A0 AS SmallDateTime), CAST(0.2794 AS Decimal(28, 4)), CAST(100000000000000000000000.0000 AS Numeric(28, 4)), 0, 1, 0.7357, 0.1683, N'OîÐCä_ZÜÜýäbAåuªß|bAbhbzUÄräßªîb|üäa>@ãA.h>ZäUýî>åðÄC>vAã/åo .ýoBß>barBZÐaªö£Ü/ýÄz*ðÃOöbÃßob£©Äao ObÖðåzÜ£ýCî +býå:abb<îã/+rAA¢bz~ßzª+Uß/bhArßo+/BbÜuÃöbävuz_ÃvÐAðßz_b<¢¢£~að/ÃAZaÖðü', N'ª£Ä: oöUÖÜ,aZaäã+¢:h|Cöh.|OA@ð*ÜOCr£ßªh:Ã.ãu_/.a.O+z>ÄU>Ã~AÖüðÄß/ªãOßraÜ<ý:ovÄªýoo¢*|bv:äÃüU|,>ÜÃAßð~UurO~ß@UBuüübªßz+U,*+zCUÜrb>Ã:UðÃ,îö.OüuÖuÜ:ýv@ßÃ< Ü.|uðCü,,ÐA|¢übv£Ããårvo~o_£U>©Zßuh<>COüã:Ü~>Ð£o>öýåCä~C>výb,:ÄîÐ:u <>_o:Äý/.Ã£ä>,oz>ßoö¢AüUhh~+Ðääb> ©ý.ã..äoÃßªC|ýz..öZZÄ:_Za C¢Ã_ãª+OÄoå£+Öb< o£a|~bz:îÖrªu/CßBZU:hüaÖ+Zo@Ö~@ruh,£aÖ@|+bðão|<ÄÜßv<ãAOO¢vrzîßÄ Uý,Bb:ß Ö©bbAo~ßß/Ððzb*OöraÐ|u£~+~ªoO,© ýðÄoüCh<©,@<bÜAO£rhä                                                                                            ', N'Ü_A<Ã:_.¢.Ü.ð+ ruãb_bOÄo@~|.@üUßåîooh<.ÃðUÖBÄ£ã', 0xC01802A7D03030B7458D8ED80E2CBFD0825C32BEB62F3E1DDD09F69E9171A2E4B33A7A25A2726C15982C41BE6B344A54CC53184AC9477891DAE4CAC7399226D4EBA925D3B2992D10EF1C2589ED80B2A8BEA2D23D58581A6CC1E84BBEA7A5DC06066CD3C10F77A13226075AAD21EEA35A32210896BFE87B928EB811CA4D6F69493F1F8380ACF09847009F1650FE6D4AD345F18B859D45461DF141335765D7E4B79E106E026B22AD4289C0951FD12A94216D3E855CE3E3ACF9D8668F6BBC41E129BB5FB2EDFF4868224448FCDCB0264BC6AD12E9275288D3BA6474290492418547DF6C647278A62225D662179BE6D049B21AC9BEAA58BAD7338BF7898A1E638F02C2B5FD43C2D94E06F0C350EF7038D3932A77EAC983DBEF249531E6801D970773D314D831D305EF407BB7BA946AE7C25323F55FA678C57FE7C5D9AE91F2CDE958BCDD1C5B5FA2C02587A94ECB215AB9AD6E935C273760FA0D95F16BFA575E4EADCE61D88DCB67F381353A1ECF77B4C56B1F01225F71C2205BE20FC66E36C6CD295BA0FCBF822CE207A0DAF1FD412B95E02B00DA563CEF3C0C407DAABBA0844B6F6CF574D6B805E9F31E1BBE96D02319B779E5420000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x77ED6B64EF7E058126661B8E82904CB6BF282517E7372030A53C1D63F342BA421583F5080871C886E472F8B4A2074D93CD00B9BD1C1E0440CC3BBEB361A18DE3ABD979FE9B43AE4D609E32743E6B8D9EEE8995F288FD25F67FB43CCCB2142542E41408C98E4FF88A526C5C7DEC8F0160AD20C5D70B9A5AAEB4827FE606E19A4C79EF902A63C40F9E910BA01CA0A70AF0B2DB1E924E4D364AB95A140BD0118370D11003FCFFD7FDE910264500FA3E0D0C778F342BC208FD2CB9467008AF03CE80872F4B90A30EF0DADE35812807364F7B4C089441DC58AEC913D742B967F43A0C57C578CAF27C69699E594C8D86D42483F6F4BBA76F8DA32F81827DCBCB3772EAFA74D042BD8589D09F2AA3830B20423EB3BF, 0x38939CCA21494970FF9D9E66E661921204E2C6706BEB0845EA202360403F180B74FFB267D70621733C18D0018C3C9CAA8E2F39D1F951084C8B57DF0025E830BF8492F1E7AF2B3AFDC05234521A06B8AD8379D21005471222F665DB7F26204E53E8107DFA4AB5FA62F53C10A71DB631859F317DAC151FBA3C80CEF04A4FBAD7A004E0690989BE63CE7F6F1BC9376A05403356E0F4A3A13DC856407A7CC5B0133A6570A33C4C27FBE1CEAF2DA604F10515C8AF16F8DAE2FC62A2A1B34844E709CF5BE064C7C7C9F40C5AFE26F47497C3653B64B315991B6082734099AA74B39DE7A65E4A248D3E4DFFBB099DDBBC08ED0BDD4E0DDF33F3354080E153862F4998A7613E4E164C9FD7FD16B359907D84C66A538ECD25A5BDB3B2280AA4ABEAC7CF7271F5797DBAD7BE3E7610C88099A5C2FB1AE933A17B7202A48B9BED42FB2C134FE3E71E870B48AF7CCF8FAF95F433188347A89100932EF6FFF3D5F21EE97C434AFD799CB2863AF0D239B7D2ECC26CAF578369FB529A12BCB5F11E9435569092534C65987B9666FD0CB8BEC7DB5C093D87269A9B7F80F0AFEABCE6C067A6BA7BA8257EA05DD433C11D6CEF99DB4BE059E8E10D0FBBF513D25BB5ADBEAFA59565FE431EB6DD69DFB2F2D97D129157D39FDC8CD88699AAC0B211AB44A726751177D7899B982C2A7C7E025A1DE277004104E20DE223CD0FCC53836BA43124388C7DC8063B07BC3A73B21059843E8DDD3B116940C41892EEAAC8BD4715C0854A59C9CE9F1A71C43A47A79A3012738A8DC5A53093C7E47373EAD95B73497424B22CC500BA70EDC3C94D2FF81D7E62DB373D30, N'oZbh_v.|ýüÖCua©~a<ãB<~ðbüub ãbZoðA</ßCäCvbäOã/ßC|Z.Ü£Äu~+ýo ubÐö©ªOO@uÖîªÄã.ÄÐr_/ðÖ_¢>>ußîbãö_îO*ð@UUª~*_@Oöåßå¢ÜªîZzO OBü+¢hðöbß£bßuüZß .aB |*ýA/b*£¢äo~ hZUürª~hhuÖB.ßzBöîouä@¢,ªÄäª,oÜîråßa|+r.@O/CuðAUA|Ãüý*<¢£ã*>ÐýÐü/äå_ýz@|ÃÖÄ©B>o©Ã¢~ã<ßîä,r*ÐaÄ©@::©+b<vÄbCvU¢ß@h©ÃßC.å>¢öAhÜöß|üBrr£ãßªr*äß*_©u~Ãub î|C|öb.A*<Üv|Zh/ªäzåb¢Ö_BobZß|BÐÐUb_b£Äß+~ãåb@ÜU@ßüßîOUZß|:', N'hUß+b@ uª+üo@><Üä<~+ßÖa~Zß|ýßÐ>OðãU*b>Aa:Aî/a©,©Ö@öBå.h+urBuü /åäÜUÃUhÄ*/~ðßä||ª/åUvÜ.z>ÃäCä.ßACªa|u/Ä/¢ß~£z>*@£¢>ßBBoB:<Ðý+aäö¢_ßOãa*OÐ:ª<rÖýa**:uîaða:ðCÖÃÄbªö¢AÖÃo©£Zzb£>ÃåÃ<Ä¢A|¢AªªßÄðå~<a.a@~Äu rÃßðªöZ@ÜA.výBAb ~.oå/ÖÜÃãbU<ozüv,/ ÖÃ/£zZöî: >î,örazb/ÖöAUÖîbAªÄ/a~©Ãz¢hÖãª>ã +*hä@üZ©Oü©,Ä:ÃÖvU OazîUÃ£|ä_ußUBÄä.ÖO>£/Üß¢ýz~o:a|+O©zb/åoå,.höh/ZaªvýbÐ©ÖU<Zv,b..@©:b/,ßüåA¢Ua¢å<|a~|OA<¢>ðb~h¢Üßýa¢Z©+ãªßZCbB/ÐªªßÄ+>ãu>Oã|@ rãUUãvýa~ßroüÖîî/@bO:ü*ð,zhÜCCãß vª:äßvoÖªAý@r@B ~Öz>*åU< ã@Cuå<ý,ãÃh/A                    ', N'î/ãö*,rÜA:@*@zý,+ü_öÖb_Ä_aC+ãüå_Cb~îAzhð£ÐBÖ+C:BãzÜ:>©>äîC, Zßöb|üU:Äb*bãªzaöÜ*/_a@@O/b/|Ðaðîß£ãßAr,åa,ÜüÄ¢å+>îCÜößvz.åhðßîüb/oAü', N'f29f1155-21da-46e4-9f9a-3478afe88bf7', -9223372036854775808)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (7, 2147483647, 30454, 138, 0, CAST(0x001E16D40172C9E0 AS DateTime), CAST(0xFFFF059F AS SmallDateTime), CAST(100000000000000000000000.0000 AS Decimal(28, 4)), CAST(0.8320 AS Numeric(28, 4)), 0, 1.79E+308, 0.3759, 0.3758, N':bC.Ä@|~îîZü_ß,U¢_ä. ÖÐãhoZrAª', N'Ü@></ubAðrvoÜä* |ÖÐý@ÐÄÄ*vv|U*|zü:Oazîß.>ß<C<|bZßö£väözbßÜCoö<ß|£üOz© a£ö <ÄUªUßrýÖZÄz:Ahhu_Ü£<aAð<@+Üª:öýz_:ªßr_ üöÐ>ã*@oo_C<äzã<© CÐ./,Z*Äýî|_*aý/©@Z@bhýrªb*:hýÃýð:Bßð_|/oh*ªª¢rB>.£ roUýÐrß öî~au|ªåðO@höã*öÜ,ªýäUüÐ*+ßÜrBbÐÄU>v|*,Aazozî@+*ßzr*~ã:Ãöä£ãÜã©ßCÜ.Ä B+.ý|ü_b©ßßa/üåîÐhåÜß ÄÃ_UO£ OCC£AÄöÜ*AäîªZAãã*äÜð,£@ð@Ü©Ð/zßîÐ<ÄÄ*.vöC+CAärb©äÄîzä,ß:~Ð>ðvbîÄbaäßzC>öªßßa_>b ÐýäÐ.>A ÃUhªî ©b<.üã U+ÖröÄb~Üðb©orZroZ|ý@CbüåAZoÜ_h@ra<                                                                                     ', N'_:ßýÖî bðÐC,ãzC.äßhzÜzî.åßUÜ_:oÐ£B¢ßÜaÜuÐîüzoÖðuaC*u*  ', 0x23D18F7C0590EC41D612B63B18A6CE8FF4056BF8EF8EEADA55ED1A946D150839302E6EED49D3577B913DA68991D1E2EA3F4B57216FD4B595E6B527177658D3C10A7C0BEA38E04521AEF066B1E76149830B2F5422BDE6CA5B78D90694AB3F05396AE57E14C6F8B25E0536737E6E04AD856BA40C4603B00323B2AF715ED9B0756FEB2D4701E235BD127DF61C87734BED742B81290732BF35B9E25A611687538455C0049340BCABA335E7FAC80D947BB067C2FD3358876D3C2BF88E1F87D3A68B2A71C7C76EB7ADB87FDDF32038F6272567AAFA5489202BCE52C767472D8C1B61406F9E9CCCB441AF1445B37A934D0435D069B7DDD952010A45F974A64F658AF3C291E0C9588B58B3684A3F446CB03A05C33277C3D53E88EE6D93000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x3F9A60AFBAC0B8026C3BDC451104748F12BD9D94BE6EE31DEEC61D8409F15BC4348473CDEA8F89ACF83799885531B2D0C9F8DBB0F6C215D1B743C1F4A234F97BDF18108C90A7A58233F19C86D55AC7BC134DBB1015ABCE726FA216A7CE9509C43E1DA93D2025402253597C04268EFC5D7B05E05579E86534499E1DEE90FEB49EE7D848B7B0899BEE0D99BCBC2FC94BE1A5B82BC8B4B44A55B875F48108519155A5DD1D4E4544ACF16AF248E02BA90480533B48D8D2EBE9338DF2FF3B9519E5BB08671751ACFD9B610C395EC26B6A290F70732ECC08B882B9F4E9C05245705C500C8CD8C491B3D355A0E7CB998C26C722C9EFCB21EA18B34FE55D5CC23C481B9BDD0DB5DF92657DBC9FD92C20EDE92DCD229E8955A53A66F3D988DF3457111857CB30BB7E70E88F63720E8111EC9261FB308EA09EBEC6F5A2A7C205098648648EC0D455BC2DAE5C390E26E296B855074727E304C8707A99053A18D7AEE386DD29AAF350183CA4CE8131678F98E35FEA52BBBEE2A3, 0x1D463B4F8214D6AE568848B9ECC504EE0A8214829609EA0022DB2E85F325A33AE50DD50ADCED22B9AA831107502A72254DCCB3608EC105145C3B2DA7E551BA30E50ED3DEEFF9111D5F5BD712F8E2B0F7690937277142AB2308C1AEA1BF7BD359C9E0224156933554BA960033E9F21B798B363EDC1A92BFA262B0D62E4B705EE4877E4917C06EA1, N'ZU¢öã¢o£ý_BU>Ö@. @£a_,öÐÃîbãýAü+:@b©:r@îr.ªÖuaÐaa.ü¢ý¢zîZåvö_ ©ü~ÄÜüÃäühaÖßhÜO>©obã|b +ãuO+|ð|*åÐ¢O>uÜ h:ã¢UßoCC,~ß><Bðv öÖ>>,r/.ð,oaý/+ÐÄCrÄhabbvO*Üä,¢ÄðOZAß¢b£OãîÖöB,h.ÖÜýÄ|rðÃ/Urä£_@Ü:Bßo*ðbA|@zCZCoÜr+Bª¢ý*ü* +,äß@Z+ý>ý~ýbãÄüoAåß. b.£Z:ðh:ÐãßA_ÃOÖaAÖZªOruzvî©ß B_ðbz/huã:äbUÐoAZAÜ,aäbCÄîu/üOA:£,ÖvC>+Ö¢Ürz/|ð<bZüäî/ U>ß .uÄv£/..ãüîðß©AÜU£ÖÃoüÐ+©ro_ub@Üzoö_ð>|åaob>o+Orå£B~ÖCO:< ~/väOÜ:Cª¢Ä<äÐBä©aåuuzaü©|ýöArý>b~ðCªoüzbAÖ|<,ß+<å/o|vCZ.zr¢Ü', N'.¢u/vßýv>A~Or_:<vÖ¢:oÖãöß<¢Ãzü _@ð*ðvü@|>ªöhýbðÐü                                                                                                                                                                                                                                                                                                                                                                                                                                                                               ', N'ähbö©ßv£ßOZö_ÄA£vr@uzubö_ßÄöÄ_îÄ>A:,:ý©hý Ð+r©,.ö.©@/ª¢¢bC,ãB ðUZ~:@U¢Ov@,uý>ÄÖå/ZýZå+Zå.ß**~ßb©,<B¢räO U,ýb,¢h*+@.ß ¢*ðZ|C.Zã©Ã _~AªC©|B©b>+u>ÜÃýýb.u*ÜhO £ÖÜA,ÄzÄãbCÐUääÄ£|ä_bb Äzä¢ZCrßå/ßaß £©ßb>©Ð~UÖr_>Ãuv¢U~/bÐ_Baª¢uuz/ãb,aåb<~,~hbC/Öª@ª¢ß_/+ZîhÃbåb>+öz*<C© ro©Oãî©.£UaB¢ª>å.ÃCCÖ+.Ä BÜCßBv<ãv.£åU_+uÄAßäöäÖ:/öÜa||ãÐüãÐh|¢|ü_v+rãýohÐåA ªã,ua©+h<:vÃ|ðð.ßU£b<ß Ã>|ª/Ü£ZÃ@@hh:@Cîv/>bý©¢ðbÖ >uüäÜ>ÄÄ:bö*ãO~OAÜßöZCÖßA vÃãðÃ+AzÜ:|©*¢Öß©ä b£å.<ªhvßb+<hß/uaä@*|z_,Cª*/or<:å>üÃ,v* |', N'99999999-9999-9999-9999-999999999999', -1570628528)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (8, 1973142402, 29235, 132, 0, CAST(0x000A8052010A2980 AS DateTime), CAST(0xF8BB04D7 AS SmallDateTime), CAST(0.6040 AS Decimal(28, 4)), CAST(0.9025 AS Numeric(28, 4)), 1, 1.79E+308, 0.8312, 0.5042, N':bzühO>>Ö:@ +Öuªîý<bäa£©@+ßAO~a/öÐZ©ß>hÄÐr*ob~uåªhðÖACî:/¢:Ü+< o:B+¢:|ß,ãÖ ©©>ªöoð.A©U.ßããUob+*väOÃürª.b @îvÜUßöA@ª:bZ>r©_ß¢ £*<.bß>ß£Ð_>+Ãh*hb~AZh@Öåu|©>Z©ßÜÐÃo/ãÄ/zo,ßbö©~+ä@>r~A>ãÄ@ãUßz*bÄÄ£ýýîz hÐhÖÐÃð_üz¢Üo¢OÃr Äzär<äAÐ*Uaª,öuobãåÖCAªzv@©~îB@öß©rý<övZÄa Ã@+ðÃ_a| ß/rÃÄ|öüO:ßvÄbßÐÖU©C~~bCÄîÜå~ýAå@rh*:@vðýZBUObý:~rðz :Öu,BüßÜb+r/ðÐ>ð/vUüÖCZ>ßð£î z,bÐ|b/îö*_/hððîßU <@hðÃ |*:Ðªo ð:ÜýoÃ+©bA>îÃ@¢O', N'ýÃz.ßo<*.ýÐýO*rr@©oðÐCå©.>¢aaßöobh¢<a>©ª@ßZ+ðbãoÄ<aüA/ääß£ýZ@: ªb|îAÐÐraý|.ðß>Ð £<Z@Üüh£vÖ£+@+bb<© +Oß/ýZýü:aöA î£¢boßãb/vÖübß>¢<+/oo+oäbzußa©¢ Ä :äÐAå©ãUððA:î:>*ü£@åo_>Üýã~bbÜ£/vÖ*¢å:<Üzî:Ußª,ÜZ£Ãüßßoð+üAAbßOoZªOoCä:@OÐ|vBUü,/ßÖ.b©¢/~/vî//Ã@ÐZüu.U©.OOßbUîãCa£~vAÖbªüÖ|©ª_BÐbb@ßZÐüro uîå_OÃ~hîÜã.<ýu*U©©ýß                                                                                                                                                                                                               ', N'bAr+bßvÃ,CÄ@OÐ/|åB<£zî¢|_~ö>Z@O@~ßã@ýz_z£B:zÜ:', 0xBE640B5053924B55B883DA7707FA2908614B5D2064F1CA38D5268B4B387B799624CCB205CCF0D5D4A555378821977C95AB5AED97CF4692CC99D37B2D07001C3D0944523B48243C7176897B0FBA42173EA9F69FDD8DE2C6DD39E56BF974A8CDA438FA7EDC44404D871E7B2146CB831257BC3C8310613EAF277E750162B842FCCDBBD6D6C0C33AFA68A818BF12C08012B6D3143D3824BC43FA4A565ECF3EEBCEDB7E538AF4AD519381421A68487F9C66FD624B8EA405C4A9FF93DC756CDB2B2ABC49E3936DF51E088280BBF50A54FA7AADF0606CF25C2AD6BAAA8B61D1DB26A517F0ABFF9B09F90744430C2349CF9FAA9861C77C7BBE375F1E0DD9C26ABC37A65915DD19A8FADFA4F65828AF50068CEA0AE0C87013AA2A58B94BE18E30369D870B65CA33F185029E8223862296870F1780EEB78AF4C793AFEB84605C195073BFBBA00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x0A, 0x58F985DD2AEDEF2308C2313CC90D427F129344D21A486C25A68431AFC766F6E324DB27F432ACCAD9A38BE3C22E1F50423140A80D5F1821108C6037A5BC405CA1CC4D18EEE64D4C66C86B2E8F65A9F68734EE7E86BEE8840BB91767D275241C632E42F6E006625CE4F34244F4E992AAE36AAF71CDC1D5E3E3C90D34CEE57AF0A44ECB6023B31580D1919A937BA49021266C8467C5B24EA1585E1217F92D8EFE7852A0A61E31872F542F9F3C2742673F39EC0D6260122AF1FF2A08C4A0FD86A4039B1A2904E903F5E455129512127916F7D3DE0D1F2C9D6627AEA7F329C38E56A9FB388B283D3A4FE9187D0BFF94DEECB0F1CDCC1F7885D7FDB40255420FCC21C6BCEC17D9EEEEB9998E2406AE926692CFBA4337F6C1363075618D733BE40CD94C5B4443A8C712C203133391547E286B4D708688CC942B0485BCA178607A7DB4824ABA524E22E908A78034E28EA5DBF7382226CB5E840DB7303FBEEC259A6189957F0F09C3D51D2B867DE04B604876DFE0AA3F3E54B255FE3C03210E64FDE30D1FAB4C0E96D1336205FA45E1A3C257E8C9DE0B7A5A2334E32A10CCC0B40EC68899D22CB5FB67480E76DD38596C735A7AAB394585F268B3FB7668E4850572AEF634BC47D196A1EC6196FD037DEADB76B6B652147AC94196311F303F9382C309032E4225AF2C89E40A4DE3EFF46436109AF12ECFB25E342DF22616B2656259DED2F3FF4EA8B6C7892D6E0A2DE652A1CCE81FA4A154F356A5F914328916D77EA7F3041C0064E768E42AA4A244530747A08449C945BEB88ADF9D5A5493833D7355EF6339E135A06701B7BF76AF511252F2CD9204CC0E39E93B7A3D630A92D460ED304C55F0CD656AE4E1C78DFE41DA8D8B0862C5F139A747B67AFD67F83D5D6FE25F897F04BD6125682720EB46ABEAF99FAFD802300E6FB389049E76D0FE71A68458AA9FB85B88CDA63C5DC919BB010A8F03C849655B3FD3EFD5F5FF1EEB5569B57967ED004B4144260A5BF3, N'~ð~>+vðå|£,a¢ÜACÃßîýö ©<ð:.C£ªÄäböC/ZÃ ßaão>:|ª¢ü*ÖåZðßUvB~îäßuüö_ð©©ªßý:CüBOOA*ãÜhZãß~hCäOOC/öuªr|ð¢ðü+@Ö.OöÄbBbubb£öCCöÜ|ß.£åAoÖ<ý¢CÄîßÜÖb,,r,ßBCî+å|©b/üä+oüåå,arUb:ö>hUårbÐ|~oaz@ü©Zrßîroä_©ßäOa+ýoÄ¢O,u¢©<AÄ.vuo£+îÐÐa.OvÜ¢ª~B äÄO~ýZ© ~/£@*å<Öªz>h>vZ£ýÖ_*,Öªb~hoýu©uäüA Zb<bZßAAzböã~/@ªUÖ ðßãªðrÐ>@öb£o:öa~@Ãv|+ÐZý¢z||ÃbýB£B>/<_ä_Z@Bu.ß+£.öB|>Ö|B_ªzåoößåb_>*/|CüßÄZã£b|ãöðhÖý/ªî_.åäUý,*Ãu', N'ZauãßCÐß<Ä:<b:öUßbî£öªßAÃvÄã ðo.ðÃ|ðÄZðäÄß.ßö.ßßAh|¢ >ð©rOo|,î<_|A_+îa_>hAðhOabv:ðßO+_B.ÖåÄ@h+:ß åvðä/åa~b<Z|ß<rCåC:BZOaðh¢azA+Ü~CßzÐo.aAÜªzbßãö.@.UUýÄbÃUýÐßåÄ|ÃÄÄ/ýo:ö<B/z©äüCZ©ªÜªC@~.Ö¢>OÃrobA¢ B,üUb:Öo|,+ãvä/oZðö|rCAU_öÜßåvv£u¢aåÃ*Ðo+*Aa+ÜUzUbäCBbhý+ÐÃB©üZZhÐª£ä|:ðrßrãBAäÖÄ<|ã,h|o<A@ü£¢<,bÐä£Oª/©~öðZäzå åö,r./îaO/oßb>v|zßÖ+Ou|CAåU:uvZOoObÄUvý¢ßã<Ö/¢¢B©*üb_>h©+vO@r/ßîhý©ä.| bßC_ Ðß©.voßäZu££A.*üB</+A|ª:öÖ©BAðv.ößð_~ÖUb£ö¢ð Aãýu*ö ÃOzOuu<@<Ãã*öuÃ                                                             ', N'ß+ u,ÐBö©CîðÃå©ýöÄÜ>Öz+ZÄ|üBvüC ª>.ü¢BZ/ru_C|ªßßo+a>ðÄÖ<aA@ßÖa©Ãa|zO:Ã+ã/ z@ÖvÐCÜoðîüª.åÖðö~~<Ãr_äBýZª*:aob~.uÐÖz,AªbÜÐ.h//@ÖÜvü£CüîoÐãOß:îß@ã<@rrZO/bCÜCÃãäoßÄuîCðZUßª||äßªäöåzöî _>b_|arß<åüaýÐÃå,îrZ,ªÄª|v>îoß_îv>OÄß~*ZbaCÃ~ãßäOb<,Ö¢ðöß:üz>Z¢Ã*bÖ>|übß_îu+', N'a79b53c2-9594-414e-b4e3-1e91d8603ab2', 0)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (9, -1, -2535, 255, 1, CAST(0x0000E55D00E1CE40 AS DateTime), CAST(0x00000000 AS SmallDateTime), CAST(0.2215 AS Decimal(28, 4)), CAST(0.3612 AS Numeric(28, 4)), 0, 1, 0.4378, 0.5217, N'zuý©ãã.|<Ö/Uãä|@+rðhð*AÃr>C:ýAÃ>¢h >C¢zZuýÖoð+uðräbhªü,öOZ:*Ðö~/bö¢Ã@b.Ðî¢îðªåÜãvOUhý_î©Ð,zßOö|v.zbýÖ@.h/zuüb:*z@¢b_Ä::@AöühÖª|öb£@ZßßbÖÜBÃ©zð¢ZAaz_åa|öä|öß|Z,ßÐZî.öb', N'_ ÜªÖÜrB ©BÖzAbbB|.uãZr<ÃåðAAr*ü<~ý_uörB*ä<åO£>Ärã +©ZåvhzðåäObC hvÐA@ßh+ åýßzzb+rãOåüzOÄÐ.ðazuOß*_A,Ü©©aOý .å:ßýå@£ åu+ãbAzBîCbBÃr/Üv£~@ooÄýüÜßZªöhß|ãîhOrzÖ+|CböUÄÃaAÄuðCOÄ@,åzrA>üýv@äZzªb*ßo~A£bÐbCv|uðä:©O¢ßzý_*bob©Ää.Ä/£äBü~Ãä*ovBÖðÖb,bOBÃßÖ+Zß>å~+aOÄ|C~bÜ<b>Ö:oÄ:ªo,aZ¢Ãza£å>ßOBßZühb+rßOý¢~Bªð*ª£ßr*<|b,>,_<Ã|özýÄ<<A£b.*uA©A~ÄbZZãýüÐzÄîÖ~ü£z</©ZCå:å|aÖßýv_.*ðÃ:r.Ä                                                                                                                                                ', N'U.Ö~ZbBÖbÖªýÖ:ß¢,ÄãUOÖbO<,£ZOCbÄÄöä<ÃzZö£Aö>ð/ª:äãßÄåÄh<+|Bå.ß~öÃîvß~*ðB.îªoß_Ubßr. z*ßaß+CCr,_ß/ýßAoßä>îÃZüBý©ß bBr,U¢ bUarUßîA+ÜÄßÄuåa î/¢bãîßÄZßhz /r+<üðß£vBßUª vãBCOb', 0x401DAE81CA729AB33850429453B899C20F4B3C5944390AB334F715DA8FA71D9B6F34ABB654E982728E9FE274C723A9A9E83409FF620F0F0712FA4CD25CBF249032A7241E0DE36E25C9C9CBA55796EC104D6BF25A9D1E386025A3A4599CAF15CE7A514120FE8B9C05C6E43599436AB07B0E3C85F0CDCAF2146FC4F9693E148819B3FC2A0555CC11C2CFED93B9E89F1CBAF3284B602A2501B05F2DB03A9233C6B25B1CE5463FAA25412AC321DF37392BD0A73E7516C828EE8EFB7AA6281BD72F9B26E05DCCB9D57B49020935670321782602E09253BDB0EA332D574BC404FD26539385B956DB5E16D1C32C13BEF2878BDA0180263A44DB0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0xF62A9F628AB64405E3E2D47D68224BA3CAC090919D2B40A6CB71C284D7EB0FB7A8425FB0BFB4470DE70DDB10B30D71DF348270834B4CD9CBE9F99619F3C02EF7D21CD525C39AE4167982A98F4F30F2BEFFE34F547B8C6B5B6994C7C61D2D1927DFE43D96384C1B9A9E5909C9A24A7BB806D81A0F70D97B46B079BC2FD31EF84B6881AF6BF35B85B2C7B4B62182A2B81F180EBF44130AB60668F6E5BE90ECC78061BD60209A10C4AA526747BB2359C40EC09D16C45EB4385ECBA802FD75C622DBED35C0C4429784084915F1ACC2FAE30E31D7FDF2D50222AC850155F10DC7348C446CCEED99FFCB29180E7A49C28B9BD0C5F4CB9EC93005EA, 0x3E404192FA82E06567562CF675310BD4DAC688D8EAF965852C18FB96C21466E4D985C18BE78614F8685C2345A4CF7F9992F4553E623A2944DABC65E1864AA253EF471CF07080ED53B290708DD520E15D996103CDBE27829E5B7CB02A2032711EBAD0555CB28E291107018CD1CA9C6EBADA08482546D495C7C57E94F4D0C233801F51D305CA85AA0A38F1C6BC9BB0D3BE0DF54F17E94CD8E53B8E9634FA344145CE0BB279C969F0555D3DEC540ED82C15AAD6E6DA721B96D378EECA3BEE761003B081ED8FFD5AF5A405FABAAC5F84D66262818A234E66D2B275DF00DB9EB466E72CE8A38DEA9C174FE141D57861E973F41767D96DECECB549AE3B1A1E5919055193D28077E04977C5693A420C915A518B6C0CC336DAF9FB3F325ADEAF82F8F5C73B84E58E69FB14CEF01303B1AAA8B854E98EB24D55CF769AD74144856AC84371457BD00D74C22270F7F546C6F549958AA28AC5F346A7716C0D4BB628B78E6427CF926C2ED8ABDF276B42566088AFA48EABBAE3E002502A9D0964FF290BFAB869911EF662DD66B2F6AA9E87035111E35DA6287CFDAA84257F67CE2473ABB51B7189ECCF6AB942B14508EC3563C8274CF880A283E27DBBBE0DF976FA0E413E4BF483157A644279A4637737F4BD4EC1B740822CF3093238F3CE2EC1704247A203CAA140E0B2ACE7B607FD1F6B46C601BE38BFB98B154A42C24A22AC368009621CD4332A1C0EE004939A76EC0B12CDAD072C0338828E17EE7F5F7D969000B135FD9B0396129BE5B057BA25F19E5E36AF7C75A4097DD0DF8DBD45ABBC904A7493E940EE700F379A6B4753E0C768254F2029CD191B5CA7B82DBB75D8CCF0281F0D4692075E70C5A045DA39C229F0172597F68A93C33059364188C1B5995ECD7B1F34D381D51EF0B0632CD44022E58E3CA34F40D665945CDDEC5BC6D5AE1007E70DBDA2F51D96507905DA2F965C18145BFA8411491DF7168AD7F59479571B025325EC336442981F0DFFC7D08B64C581D4476ADB29FEBDC916A7181E2701D621EAD8DC706FE5AF186165044A56268B251174C031ED40D85468221CBC2A7AF80B28A5E6B3885511EE975179C4FA7273EF1CF20CB241A434AAFF88074CC8F2EEE76CF821B03FD89DBC62D1999BC61359F5AE2CA0386E45CA83703CB3FF4DC525A56391580151C3AF44544F506F0A99E3EC2E129D9B52B679272E9D3F03249D6E9DACF43734BB57A4EB7C5BE31EF55A78D758C6D13B278952AD1830D3B383AC1FB7E0B2C31661AF31D66EB21EF6D35A31D42467AB5ECC337522A18A1B8A84F067D03B2C3D42976276A995FBCC756A5BDABEFEFF8F1C00C1EBAD3DA174D693A6517E4A4EBBA61CEF3CC1F101C9D066FFB746B05D8B85614BCF7A419004F5EE9F16EEA94E2B1CA033A10FC2CD2AFFF81A463EEA2E26351ACC1CFF7E40658CEA47D351EBDA27, N'z.ß£|îCýÜoî|ªã ðå*©bäßÖ,bäzß>ÃU:oaÐßßÖ,Ä£v*Ðb|äuab|~üý.>©.ªÄªZ<Bî/,ªC_hhüü/~:A <ýBß/C£<O~uh+oÃB*ÜUß¢ovB<ð/¢a_ãb¢BCÄ@>büvüuaî,ÖüväbßrrÄ Ðý >Ãa:', N'o>ZAý~ãÐZ|©ðvrbO:*o<Aä.,ßü@O_ZU>ªUÐî<öZvuBî£A,<ßU u©zhÐzbîUab©OazOza:OÄ@båUööÖ*,<Ððrb~O:>/ß|Ävu|îî*äb:*b¢Ä~/îß*z_ã_ ÐüüÐýä@a*hßß£©Ab.< A,ä~*ýÐ|ÃB@AåÐåßööªäuö Ä>îÜÜuÜzåb>@öå,ªÐvzßðãBðbÜäBBß¢Bbbob/uö ðÜ¢ü+uöö_ åªðC,ä,üÃa+©rB/,/åÄzO@züîouh|äãð_ÜðaOö£ZÃ<@öüU|@_üÜbuUo+zöuU£v.ßo:h/o.u<U/¢Äý*a£~Ã~~BßÃz¢|ÄuÐ,£.bÄÜ>Ch+CÐzßßª:,öð~©ö©ßÐ+ÖßßªÄýÖãÜr£v|>@: <äÜýåÃooÖZoÐÄÐUü.ÖZzÖ,Ð|üA,ßC+/br¢rüö,äC£/üÄª                                                                                                                          ', N'<.zoÖUh|bCuaäZ+b+äB@B_ZAaåO© /Aüäöð,ß', N'99999999-9999-9999-9999-999999999999', -1869167400)
INSERT [#AllTypesComplex] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint]) VALUES (10, 1550604226, -32768, 71, 1, CAST(0x0000901A0000012C AS DateTime), CAST(0x00000000 AS SmallDateTime), CAST(0.9869 AS Decimal(28, 4)), CAST(0.1231 AS Numeric(28, 4)), -3.4E+38, 1, 0.4740, 1.0000, N'vhð+üUoOZ:A./z£î*,öäO:züüv_bßÖªã<Ö:ub:zh.£', N'ü_Ãü>üÄBvßîßãü,ÃÃB,_ärð©ã/ßåbß/@vra/~||ãA>|Ãä£+Cbª/ v£                                                                                                                                                                                                                                                                                                                                                                                                                                                                          ', N'ªahbhßa:*+_£üöCh_|@*£ußrbÄã:+CvªüUã|/>Ãr>B*>BÐhB>zÜÄhåC,uðA/ý|BUBöÃÄC_Ãbäzä_åzBz|Ðöb _:bC/*>:UröýB,ob/ýüªÐããa|>îßÄävªbð|<ª~©OO~ªCð|ßîöUhÖý/~Ã¢b|ö/ÃÖ,ö öOãÃ:ãzÜ/ouÐBaÄaå<Ä@ª¢ ää_AÐÐ>ßuo~¢¢bUßz/ra.:îü|U/ßZ©.Ð ãüãîß~bð a|>+bö¢©.b,ªð.ö~<<zÄ__ÐäzßC.î>äobB:*:Oo>oªU* Äî|åzzBÐuÜ:ß£bZãuÜðuU@ãüªhz ß_öz,@~å_@~OÖ.äzý@ßBä©öã CbObªZOaÖö©ão/C<.ZÃOÐ:ß©@üB u<hªCüv:.ßöU.Oa<O<ßBÃª/,U£~©v@|¢räßZßbÐOo©b¢ /Ö|', 0xBC871C0773773D24D39CEA4074EBF0FD83531B6B4B28D670D44C30E8B624A25B43C8FACC14139CD4A37847FAB71C24CA462D2C9F10DC5394B0AB332747546EAEF98FFDABF123E86FB747C8D3E0DCB8B82766A288F8034A6675651C68E0748D4F099900281F79D82245BCE7C81480B3D3F27A00E0B1E6B6F54496368C077F8F42BA79CA47DB9099AE8E7F423B1B8152F5B148766574AC9D22E30AE388434C1BC52CA50E9AB56CF862E1515231A37ADAC24440DA000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x2ACE0DDCA764D0AC106B41647D0F7673B73F4450138422596B7F6830214302715BBB02D701BE0E0827B5D0E610CD266B634F8D01956EF5A6ABB47028FBA08BCD68D008C10C9F71F8303C295EB33C489D59FDCDD1B600DBECC65C2B4C4DE60D862A037D40C4645EC3DD94D0594597F26E6BD2CFFDD2B91768F41BFBB07324AA4A2FA233B0BE08D9F49E09F3212F82A72E69BCE17A160FB371EC154BA868AE50B8E9852AA36342B062F9DA5EC9C5F5387174CD46B64330341FEEC2C4A9BC308CF0B93B37AA5804CA270C7E813CB0EAABF6DCDA349A689975B9510AB4A17D06A6C84D6E108ABD8E1970021EAAC4AEF76F0416CDCCE2FCF2871FC57E584D4F29A906D8B8EE41D920384EC0C39849EFB00B599BB08AB039C4CC5BE7F0C43F35CB21D975A3ECF6092D2AF5957D2138593E3014, 0xB29E9EF9CC13EC935AE95BF919F5A2BED44BC414506D85941ABBA29301A9BFB0CC3618823B7BD7D80A75D341999B642C959A69E5505EAE451909DE10715891B129AB2CE2DC20835874EB0ADA9A44FB7F5775FE663FDA606E88D268319DC667B2E42117EE6F8250751710706453D3ABD40E5FB9FB5A2A714F4BBD5A09B752BBAD8358F6782D0C0FD425884F2E69FB1AB42794233D0D6C58B9AFC7A2B12846BDC555C3E0FBDD33A6014FE4D5D1DA43E550A3A5E1222E7DA017003CC8656A5EACDECB8AD9A61A1FD7F4EE21ED289686DE6CE2520E82649A96D722C9FAA06A4097F138F60550693E3583CEED340D907781795E5B83184F2A7A30F4018451253689AD8AFCF09F818E02A31C8BB377658847AA4781420FBD7FA10E7CD45B67BAE45E986487839387E1F8CF58E0FBC47602CB6BBAD909A4F37AEF0B5AE222AC329AE8382C1B13DD05B323668873B6DC83DD977A09EFADE1DABAA4E9977839A5BE4C9DBDF7C3975AEFED6606B6A31B80C413877D4AA8A242ED075D727629458D797EA73CD5F8E5BF453D8A1C89A62B379E242584F0BC5A15A95AD0F06624E4C6A7D88C37FBDFAACB2DC7370D1E86B946DC26D45203C19DBD2392927B28F4592059C474B019F33BBF795A5628844DF723A08914405091C2E63BCF14694C2FA3386E17B45A12082DE8EA14D8A96A7438D69A6FDA9EE018CFC7018D7449DED2724AB61EB96D999C11985B2B563CB8197CC7973DCEDD5404585547EC598494E30917A18EF24CBACA3DFE5E4242B5C2930EE811ADB2A10395F6B2C56325AC438C0378213E9A2A26300CCC00D07F11C1A292B3DE7072E73068140C98408656A591BAD13ECA9384F9E4C1E1A0E6A03316E4F023D171CDD2191C47B8298D92E5AAB5276F16AED2F70E0DBAFAAF914649C8E1F6B350638533C3E7B72282DB5B6B51DB36EABF301C6811F914C3B7D32388D5FEEF4897DE95462FF3AC2EF6B07433B24FA8D35A5C8A97C48DA2564FC27BEFC42F4D84EB12D37246F0BACB84EE716184E010682B502D797357840A3C3EF76D78BD6AC762F0489F7B358D6AA1BE13D51C40D26EB113C47401D410938EC93507F84D53C684A9FF939870ECF5C3C2D7B544AEEDFC6C89BC64F8F6C58DE0B411B2008572546A4D5173413ED64026CFB03FFC5A4FE0DC399A6E90EFCD8EE371B3E35F3AF53D50834BF65C7ABA2EB37B3D29458690856531D5C6EB03C37B7FC38FCF8AF2E1EBDA73EF790F5883627E6CBB15B1A4495DE02E7E2D642E524CC0CF7799EAEAA3C09A524FD19717750CAE73477E47ADCADB7A32EF5B74B387AB21A2B840E0E97794C9FB7B5F5477C6C8B9717FF0E7D30412059C1FB8FB, N'åð.@O: uð£ O©@hüäÐß¢|ÖUUUb_Ãbð_Ö© ZOÜßCoövÃUãÖB+v¢¢ü|îhh©oZCð¢oýB~>*uürbª¢Ãzbý.<üo/.Z~OÜro>ª@aUzBÃovvÖoz/~*ðußbÃÖßhÜrÖÄ>,Ðhb:~@ª,,u+ãouuaCüÄUU_,bßäÐä<B ý£ä_¢bääbÐ_ß~Ää~|ðÖBÜavoOÖý¢ÄßÐðZå,OzÃªðvC£>ÄÐß*üvoöÄa|ü,Ä*äbuhUa¢_Ä ©Zå ãß©+Ö/:ü¢©/£ßrv .ß~ ÐãAü,>BObAZüv¢årîª¢vîî/|Ãªu.v,ðäÄ|~ýßÄüv*våª+<+o b/ßO .ã,<vßZÃîÃC/vÄAå,O£*î£aãOuubbröCÃ+hÖ~ /Oý©*O>ör©h:©Uoß/Öß|ZA|u', N'ÐaÐî|vzbÐ:+Uzß*UA|ð¢@Äöäö+öAb.ªÖb£:>rzBähª>Ã:O:ýuAaazýOAbåör ZA ä¢O£+äAÐ<b<ðuU.Öðößa.ª/b:./ *î@ä|oãßaßª*aZ_v@vßß<rZ,<AðUOCrü.Ä+|@ª*b,~ÖB :ýhbO_rª¢äo¢h,_Uª¢å¢£özîüÐîîÜ.å:ObAAÜoa~ý*ßäuö+ußßßöä<©@ÃuC/¢ZßuüÄ/B+></¢Z¢Zh:Ãa*ÃBýªð~Ã+@*|b|üÃ_~å@>Zãz + |ä,+U_CBðrãªACÖÄäoî+bzðÖ||o>bãÜ£UªaßýhCaö<b>Ãüä,Ö|ýo.ývvð¢ßÃ+ªab.O+.BðÐÄbî/~v/b~ÃÜÖ@ã¢UU*Öî+ .£ :üb>ª/ZßãvC@|Ãý<_uýªä>u                                                                                                                                                     ', N'hb*A*,¢UBCz+~ýv<ð©~@ÐÃ£Ü.ßB/b<öÐv|:*öÜ<å<v:|©CüUÐhðß£Zr/.|¢_zOr¢O~Ba@î@orÄA<uBBZãäÄî_hbý+að+oö|zbß+BAüÄßªÜ¢öUÐuz< Ã:<ZBb>B£>ühavhÐ.,ÐavåhîzÜ>hÐ¢ßªîßUCãÖÐ ~ðzrã:Ü*ß@zß~:bßz|ýz_+*aÃ*UOaaå©©,. ~ðbãýozu ýA/Ð:ßvÜ.ßu|z><u~*ß¢£hrbÖu+@h ,bh©Ä ß:_r¢åU~OÖZ.|¢.ß@~Ããß,ýU/Ð¢B.o/¢ßh.hßªaÖîªäýðöö@î|_|ýä@.:zÖåvhOA ©¢/rZað~ðör@ýz<ßBrã¢ä¢ö¢a©<ãC>Ö/ã©v @ ¢|¢ýbðzå<ýZBoö ,:AÃßrª@B:/OÜÃ AoZÜ|Zaå|a Cr>|A*ßB/rU£ÐhaA_@¢Ð@rhöCß+,b,~<©:b.A_/b+©Uhß>|£_U:*ðvßîb_/uü/bbªÜývb~aaB|+äüýh¢UÐãßðU_,£O*<BuCðªr@A@', N'99999999-9999-9999-9999-999999999999', -3049069770)
/****** Object:  Table [#AllTypes]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#AllTypes](
	[c1_int] [int] IDENTITY(100,2) NOT NULL,
	[c2_int] [int] NOT NULL,
	[c3_smallint] [smallint] NOT NULL,
	[c4_tinyint] [tinyint] NOT NULL,
	[c5_bit] [bit] NOT NULL,
	[c6_datetime] [datetime] NOT NULL,
	[c7_smalldatetime] [smalldatetime] NOT NULL,
	[c8_decimal(28,4)] [decimal](28, 4) NOT NULL,
	[c9_numeric(28,4)] [numeric](28, 4) NOT NULL,
	[c10_real] [real] NOT NULL,
	[c11_float] [float] NOT NULL,
	[c12_money] [money] NOT NULL,
	[c13_smallmoney] [smallmoney] NOT NULL,
	[c14_varchar(512)] [varchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c15_char(512)] [char](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c16_text] [text] COLLATE Latin1_General_CI_AS NOT NULL,
	[c17_binary(512)] [binary](512) NOT NULL,
	[c18_varbinary(512)] [varbinary](512) NOT NULL,
	[c19_image] [image] NOT NULL,
	[c20_nvarchar(512)] [nvarchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c21_nchar(512)] [nchar](512) COLLATE Latin1_General_CI_AS NOT NULL,
	[c22_ntext] [ntext] COLLATE Latin1_General_CI_AS NOT NULL,
	[c23_uniqueidentifier] [uniqueidentifier] NOT NULL,
	[c24_bigint] [bigint] NOT NULL,
	[c25_int] [int] NULL,
	[c26_smallint] [smallint] NULL,
	[c27_tinyint] [tinyint] NULL,
	[c28_bit] [bit] NULL,
	[c29_datetime] [datetime] NULL,
	[c30_smalldatetime] [smalldatetime] NULL,
	[c31_decimal(28,4)] [decimal](28, 4) NULL,
	[c32_numeric(28,4)] [numeric](28, 4) NULL,
	[c33_real] [real] NULL,
	[c34_float] [float] NULL,
	[c35_money] [money] NULL,
	[c36_smallmoney] [smallmoney] NULL,
	[c37_varchar(512)] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c38_char(512)] [char](512) COLLATE Latin1_General_CI_AS NULL,
	[c39_text] [text] COLLATE Latin1_General_CI_AS NULL,
	[c40_binary(512)] [binary](512) NULL,
	[c41_varbinary(512)] [varbinary](512) NULL,
	[c42_image] [image] NULL,
	[c43_nvarchar(512)] [nvarchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c44_nchar(512)] [nchar](512) COLLATE Latin1_General_CI_AS NULL,
	[c45_ntext] [ntext] COLLATE Latin1_General_CI_AS NULL,
	[c46_uniqueidentifier] [uniqueidentifier] NULL,
	[c47_bigint] [bigint] NULL,
 CONSTRAINT [PK__AllTypes__76CBA758__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[c1_int] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#AllTypes] ON
INSERT [#AllTypes] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint], [c25_int], [c26_smallint], [c27_tinyint], [c28_bit], [c29_datetime], [c30_smalldatetime], [c31_decimal(28,4)], [c32_numeric(28,4)], [c33_real], [c34_float], [c35_money], [c36_smallmoney], [c37_varchar(512)], [c38_char(512)], [c39_text], [c40_binary(512)], [c41_varbinary(512)], [c42_image], [c43_nvarchar(512)], [c44_nchar(512)], [c45_ntext], [c46_uniqueidentifier], [c47_bigint]) VALUES (100, -42747034, 16224, 255, 0, CAST(0x002D247F018B81FF AS DateTime), CAST(0xF3880484 AS SmallDateTime), CAST(0.7107 AS Decimal(28, 4)), CAST(0.0888 AS Numeric(28, 4)), 1, 1.79E+308, 922337203685477.5807, 214748.3647, N'©ßvabßUvbðö+/@ÐböªBü/U:öv,häÖ<ßCÜÖ*r<ÐvöC~uüß >~b>Z~Bîå/*zÐÃ©+@~Bãª©o@~r oããa>oöå|ÜvruAååðäOa~ZÐb¢åüÐß._îbUöbA.äoßßBªv+äßÄßU©.rÖäBÖ,¢Orbhu*CüBßã>aarßOhßUao', N'oå>å>büvAUü~Äü                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  ', N'våÄîöÖ£.ýöÃüöhªArbý*ãu<Aä|<ÜA¢buÖîýC/Cb.uBî_+a î¢ö,©üAbA.Ãb+vå+*å:rAh<~bü|CÖ ð£ãU~hBu,UÄãüÄÖbÄh*Ð_©îßZÖh* Ä.ðzÄ<brCÃÐvh<u<rOÜãaU.ÜAÜvÐAößaÐîÃãß_UuÃå~:ª:.OÃ B|+>z:ýhZöu ooäÐ_uß,©£ð|oC£üO~*bAÄäv£u~uã©vzý+Ä.~åîr,z©/¢ß*rßAüÃ~oZ:zªBz¢~_a,¢|ß.U+Z©h©Ðb.¢ßå,.ßÃ|ÄßbvCýßª|ßÖ£baU|Ü|©©A/>|.åýv/UoÐzãîÐÄhC+>b~äü,+ª|zªãha@Z >äuý@O £a*rC@ã£/ZOüä¢~a@+ÃC¢ ªZz¢,rZî+ýåUröAo>b<ü,äã|.ÃðãÃ¢ãbÄz.bî<C¢*_îOBª/¢ü+å¢££öÖ/ÖrÄZ/rÖoß_å_ýUaöãªäö©:ßvZä Ðr.b+©ªoZ.*ð hAZb|BýCä©ßbBßÜZ,/ý~åaö,<>å+BÄ', 0x9422AC71E97870B19CAAA5DEBE55DF2E142A063EF73B0093BA81110F810BF75052D3E4925AC33395DD16EC0673663C81A7E765FA369545592219B76767612F91B3558CEB704CD351F77161E04EF91F534590679AA6FC19BC8E8B70410BCB2E7CE65B531DDA51105A131302C534640A28F96322D6F995EBB63CCF5DE5DC92462B42E5F9F3A60279F37F0D8792FA538829804847A7A9578251709CE10AF42B7E80CDD6824F6A1781F7EA2E57EC0E59BCE8473365944A9E82EB62A88209216FFF548F06C423D038BE68C650BD7AFF954217E0F8C2C0C5EA352F5497ECC29478D57A2F035D089498170E74FCCDFAE007EBCD666B2829AC3E8E00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0x94CF394CFCD44D7A424BFC383A8248E0B34CC91C3B4E4CAC1F5B284FACCEF175B837FFC8DEC178287F7F9EBBEE942CE9E048F8F1C5889546828C2DA0ABFDCE745A877F02827F02F2D4A09CBBAF90BD8A2F3F6F63D5FFB030A1B9465194D77D82B02A61671529666046061D24EBD996F2CFA3706B8E6A9E5AD74F61C939B05DFC231A0A852E557406D90E1DCEE18B44C7225382F06611F3468B0790C6C89C2795A92100D1CE10C3792E6A927F4DD0784B0A99A6E52E6C961226C4FA27C5AE3E3DE78725C08B73F9129008C2FC7F74F8BB8E45BC2EF541673C8BEDAA09169BB4802B5497C5AAB8BF96A0945425AA9993B2CC3F997FF20F47A51FE9359DC9EFFBF45442D02ADC3310D586258E61ED4291F15AF73DD602358D1B0DF2A5C18321CCB0985FD068C968D6982EC3F9CC81E6B8DD574343D2B124F21FB4B41CEA8D4E526458DDC88D712928F221A5A83F3BC19A2739C86A3FDEFAFF736DB1B3144BEC0BE0709573A31D2B6B2D1B50B00C7CB3E3F319F5B0C4E413855EDE541FBF7514A63599C8CEA64DF7090E5D3C248D4AF3F6B9CEC0FA563B46FB5D0F15C4C80BABB707AA7B72B4E53CA521EBA267A8EFF714EB5E65F1B5, 0xF952CC709737334102B1841DE3CE8041D4911AD86B99DF536E1701D96AA5BA02ED5B821FE874A56EEEAA9FB623D114456A1C915F9A40E45F7379027F3659D65CF6822F884C605960ECACEAC0FA282F9C03BC6E4E145B68AC7622AB6E72263895D442BFA1A28CF80AC23072EE5623644B49657B79EB88489B19821129B4EAEDC67454ECB8698EFA77C3648BE4E87A5652BD60240DAABEAA4BFA23871F7ACF55D742AC790137ACE2BB6B04B686FC64B39F2CBF6B54C6E01BB5AAA0BD4506DAFCF40EB3A13B00541B107E69A2A507234F9570E7F2BB6301116291C45696EE9CB49008C27749C3779FD8A45BD55D3E75AE875127D41EE741910DA23EAA3CFD8421E1B4119D45AB06D2F9481A17082BB4231B16CF45C7C896A9C73069A784CD63DB1F39F4A82DD768DB4B55DBD6CD2989E2ED1B8F252E3F5A191E68D614415F4E2CE43FF7CE370BF643ADF3FA71EDBFDB9DA787B9683E3DADE0DEDB8AC401539AEB06AFF18D32BAAAE1254B783A45E82B5E8C396CB9A5E5EEA79217F05C8131425A590E6B3DCD4E6BEDDDD743F2C5729BF5AF2B38C7E6E04620C14AFB1AC94E60F3091EE1367DEE913ECE7FDCCEA2FBE11F7CFBAD55F97A7378C84B7C825250238E7650E9A7906477C09AF9F96703787EC569DB9ECEA00BF98EAB686E32710EEB7E5B4E5751123A8F1883ECBAFF810B7550CDB61977B255ED6B565BFC27F5454F8B0E7A0BB5202CB32F4C854AB450FA0C9FF31E64BAF574CB0660ECFBB63E67223D9C2FF106FFC02FA003E92A2C0A391A5C4F5C34D6E1949599AF05543430FDF1E4630D172108059F32EFC3B70B499FFB27D19F67E1324C5C079A5857FA6BFA6F06F846C9FD21F95349BD02E71DDD894F0F0A2D6391950FB216B00F87B0ADA5C6F454B11203D6FE06D74B1D2330653F2A100848EF3E97DE70E993B24C1172A39C173BF2C92C764B390C268B4678EB81B171E576A765B5875D75, N'bA__ªBubO< U£ÜöÄbO/+UA£B*¢,ã+|zb<äÖh¢ £<åÜ@vBaBb@ßÐb_,ªa,/rüÄ,ÄªovÜßaßbÄ@ö>:@+ý _ z|ãu_bvÖÖrCÃÜ>bîBãUZbÐÄa|~îh>a/ä/+bhÜöÄÃ_vbåªãOý,ªvhBUv~äã£öoCvÐÖUU,.ðhoh_Ðß©ã.Ð.îÖîÐzÜOC>ZöZÜ_.a h*~Ü~::Ba|><îa.ð_v+*ý:<bZî£rå+a ß*Äbo£ Z/¢Zu©¢~v@*uZb£ ©å><©ßvu£|OÃ|z+Öîðbð*ov,£ ,Ã@hÄvhaAO£o£>O.¢£>öAöo~ *üÄü>ªCß*.|Ã@ãî__ZOCÐZ~Öð_ªîÄZ/_ðß+z/A@|@*ZBªÄCß~.<©ý<ªb,übüauCbüAÜÄ:ªC>Öª~,ý>hu.ÜzzßÜ_:rzð:ý_|zuuC|ö.*ýZüî+årbåãß|UîßvO<z>h©©@uAZoÖ+~va. aãåO|aUA', N'ý ,.üöb¢îuz¢:© ªbz@v|_ß~ß.zzÖ>å£b*uÄåÐÜ.>¢Bra++rãüüö>*ª/>ß¢_£Öå. £~C:¢Ðz@BÖö/¢|/bB<ß_                                                                                                                                                                                                                                                                                                                                                                                                                                           ', N'ÃÖßrbCuÜÐÐÃä*ßUÖru£.,ª£ýåvr>_ZBå,@î+©.©OZzarÖÖr¢ß*v|å@ÐbBßzßðÐU:£ß<ãååBÜ|:', N'00000000-0000-0000-0000-000000000000', -750862207, -1196886593, -32549, NULL, 1, CAST(0x002D247F018B81FF AS DateTime), CAST(0xC6A3054A AS SmallDateTime), CAST(0.8787 AS Decimal(28, 4)), CAST(100000000000000000000000.0000 AS Numeric(28, 4)), 0, NULL, -922337203685477.5808, 0.2639, N'ðv<a.,£<£_OÐ+Ab:Cß¢bb¢u©©r~abåzåÄßÜö@@~<C ª öhî@Ab/C£vÃZAß uß£/ÜUZA|vCöªUhüÄaîÐ_zbýü:ru/C<îÄO+©:îãB£C*vðhßä>züZhbßðÖ©: Abª/ÜrU/å:>ßOßB.ößbã¢*ÄChrÜ¢+/îß/@b~@î<Äå,übßoBzü+üöª¢ÃÃ <z|/ªb/_Ã+£Ö>îbz öU+©zýB:£ý<öOßß+UA~îZv _>CÜ:|<<î£vÃ:z©Oz<üübö~@~ã+vå:,b©îãOOu£îO@~zîB+b~äZ/zzv~îªO¢Orª/UuA|<<ýBßbÜ_+Zª<:z©aOöÐ*¢vZ~AAÄüîobÃbßªCär¢u_Ðr¢ýÄoä_~UZ<©ðrO.bß+ß¢urî,£Ä|oý >ßîb©~zåä/ îßr,å,ð:hãäÐBý.bz>', NULL, N'vöüäAo@ßÐî>+zz.ÃaÖ ¢Ãã>¢Bßbv>a+Ä/î£ãU_äu.£.ð**ßÃý,zÖß*Ð/: ÜuCvzÐ_ßð,©~:<åBub/|vÐa+vCÃ¢îbOr,îÖ£Co ©,|.,a.ðãäÄh©ýåbAuÖýrAðîãu:r|ÄUýbÐÖOÄ ª*ãräÜOö>u>ãÐ/+oh¢:£|O|u*hªÄBðUÃ*Äª,hýOoÄîÄUÃÐüß¢îªCÐbÖ/Ü¢ãÄo:ä~üß>Aßüah:zð©:z@¢oßBO', NULL, 0x880839E6D41290E396DA685BF056C514CF0E011CE926DF1E9E82C0C7C08FC2BA40F02EF3A5F752EAC4EDBD65390AE4C9629FFE70ECFF9762291A485251C8230617AD1A0028D26FD8BC1657FC6879E5949DF665209052CD4159CCC98BAF0099FCC9B64E8BEFE4A18F3026B3D6A79BF9B034B3B45B630375C5E0D9A65B09E329BC7D5B32B12B5AA4B855D5F06A9EF699BDD21856FB4B3D98C64089AA163D45854CFDF7584C694785DA0EFC058D130B26E93FE889F08D2D713FC36B70A2AF8E7A1050A36DA69E4D548A937CEC4F380AB33562C1D755BB94BFCF861989FDD7E32B8081F53D6F9D7F4BF194FF5D10412207B8B83B3FEDB72554BBE49E2084C457B3B0516BCD42CD4398E275E5C1BC37AE8E6412D0449BA33DC95BA1818BFF5FC72D3C4BACBFD554C4EA74A35C77D1649783350FBB72F312170D14A272A14BB89883AEB784718BEFB9104DE6D0F846C5F7F60AFD56EFA3543929349FAA33E1239EBB0AD9C91AE026F84556AB240D421EC64FAA7C964E62A9DF597D5B3CEB45862E2A4714EE6DFCDD16E4051198F8302CE615D85B88658C505960FA6F9C84E90D00D07CF9C52E305952CC66B2495110E72951742712E9FDC99EF6BA8D728D4191FAF710C77B3666B8E47F08BCFFD907085013454A10C1FA17D3B35BCE73A3E6C58AD4B85140486CF2EABC4293E040CAC4, 0x3D5F47F68138FDF9F4C7DB9745419843322A21B6C807A65360547F663693E997DFC066867E93537F534654A416527601FA9D0E9FA338EA36B8F3962DB996C361DD44B884DB11C49ED6A16281B2023C0DDD7D6C99844AF3A6D62FCE8AFD26C6BC83F3C6D29AF226E8D86FC801D56883B6B7884F3B2A29DC935011ADAB4339A1B0DBA6FEBF3B6A73EB5783C01581F1D06EAC17D374D78A925916338188447AA59FCE18262F6AC47B4D1464E4A668C7125F403FFF2B662CD95013A4A908E8EAA6DCDE214668CAA104E89C865F72A6EBF14E202248057B7938A13FD61BC61A72642ABD07C84780AA949F3B178FDFCC66AA28B35AE7C5A2A06164E8A528F4853DCD2A2135B59E22A9CE768EA8CA64EA59D5D92CF313C08A9F871CA6635EDB81A019FD4965BE96B00C05098C7137F9, N'<_ö~+Oª~Z¢,@.Ðu/,ouU: O+<î<ã:*:öZv/aÐ<£ªÃ+Ð, .ÃCCÖA. ohh*~C<ÄÖhðO:UhZuÄð@¢|BªÜAbu+ÐÃ A¢CCz£h+OîðUßråO_ßîbBîÄªb_~|bU*A/ªhãCAý~|| Ä£äß|hå©_ß£Uhî@bÜb~zð|åßrbbb@ã <_|ýörÐa.O~ßrÜ*aðªöu>ýU,oÜAª>BßoÄ>OÄ|ð|äAbOh*Ö©AãbC/|¢ß>ZoãOoh>z*.BÄª£/aABO:ßCb,BaÖ/ü,ªBÃÃ..b,Äî_äßö©ü/O£C.</*©oC/bo+b*üßaaîuað£+Zîö BÐh|~ßäüh£ avZ£z', N':@ãh©ö@ö¢OÜBo:ÐãaÄö~ÜZ<ýÃ*ÃîüÐZ ßÐ//>übrA¢o©rA>åZ/vö+üÖOAã/ U¢ýåî/Z.aZ*b£Ö_oCßÜ£ãß¢_<*üãýv.£@Ðö.¢ZÖÖÖbäýbuZ¢.~brbß</ý:.|<//ÐåÄ@îßh@ãbAîÜö¢:Ub /,ur~Ðv@.Oärvz£CC+Ä,ßZbroAåÄ<+ýo>hab@©CÜ¢ZÐ üãÖ~Cß@B_©@hzab_ã|AbrÄÃCzªª/UðÖ@|ÐuzZz~©ãîªã¢äÐ@¢~ðbv©hvo¢Z@ubã¢îý~~ÜÐv_ü*rzBä>/åBß©ýÃUåªüo                                                                                                                                                                                                                                           ', NULL, N'99999999-9999-9999-9999-999999999999', -1069124894)
INSERT [#AllTypes] ([c1_int], [c2_int], [c3_smallint], [c4_tinyint], [c5_bit], [c6_datetime], [c7_smalldatetime], [c8_decimal(28,4)], [c9_numeric(28,4)], [c10_real], [c11_float], [c12_money], [c13_smallmoney], [c14_varchar(512)], [c15_char(512)], [c16_text], [c17_binary(512)], [c18_varbinary(512)], [c19_image], [c20_nvarchar(512)], [c21_nchar(512)], [c22_ntext], [c23_uniqueidentifier], [c24_bigint], [c25_int], [c26_smallint], [c27_tinyint], [c28_bit], [c29_datetime], [c30_smalldatetime], [c31_decimal(28,4)], [c32_numeric(28,4)], [c33_real], [c34_float], [c35_money], [c36_smallmoney], [c37_varchar(512)], [c38_char(512)], [c39_text], [c40_binary(512)], [c41_varbinary(512)], [c42_image], [c43_nvarchar(512)], [c44_nchar(512)], [c45_ntext], [c46_uniqueidentifier], [c47_bigint]) VALUES (102, 1354883544, 14871, 80, 0, CAST(0x0000901A0000012C AS DateTime), CAST(0x5F6F030A AS SmallDateTime), CAST(0.2977 AS Decimal(28, 4)), CAST(0.8435 AS Numeric(28, 4)), 3.4E+38, 0, -922337203685477.5808, 214748.3647, N'Äýß~ª_îð/UCîäoÐ*ð*ª~*.Üßä_O/z>î*Ðå|oîzÄärýa~A,v©îÜ*ßA¢,ZraÐb+<¢ö*ÜOýª~:¢£Ö,+,OÜ:v>@@hb B>Zöî|£>Ð*bUvrZb  ÐoZuAåAuvßäöBßv:b<z¢_bav©a ö@vý.rO.ðAå:.ð©zã¢ÜUvüßhßÖÃÜÃoa_A|BÄaß:Ðð,*.rAîÄåÐo¢åÖbüÄCr£>.ÄÖA /äßüzAOb ä¢,öÖäýîöý+©v>Cýåªßu>©~,äªbåî/C:ã.O  o/ß@>ÖÜ£©o*Äü./,ßãrbaboßCý@äðãäzß©ÄoýÄýîßÐr/ÃOöaÃba~ýv Bvý.@UîÐßzbîzÃZ,BãZvvåÐCðb@Oãßvz@ ¢åaz<©åa+h¢v¢z,ÃîÖÄabr_å<:ÄÐåaAÖ,üb,+ß*ª+U>~<©/', N'oî@ª@v/ hãAüoC+Cåå£UýzC.B£ßÃü,ßA>öÐB@oäÄ£bUöz£rCh<ªßÜüßãßB>ä/Ð+ÐAa<u,vªÄaðªðboB::Ã_<ßaU@håz.©avîÃ@ðzOüÐåäåÜã+äÜ£aA.ÖOåöAÖöB>ürba©Ãu_UßZ<ü©¢îu,hvC©.¢~@Ðß|î_uåuî©ßÜOÃ£u,rÖðÜ>vAß+ßß,zß/.oö.ý vbªÐÖöB:u/¢åvrå©ÐU_@Co.bãªå,+AÖuUböÃzAÐ ö o+O@b~|Ð.Äå:uB©_îobÃbOÃ.¢rvO*Ã>u~ÜaAåz_zß¢ªb ßããAÜh|vUvüÃÜîä +,Är©ä:b¢©ªªÖå:~£bß+.o||ðö >b C.Ãü:Uåã©ö+Ö,C@ü@+ß+bÃb<îußoZÄ@öAz_OîÜ.uÃß,åý£Ö:Aü/üÃ©v. rbu.Ð_ö..ÐB|*bÐöªÃUZr>U:*                                                                                                             ', N'Z|U*AhäÃÐrßüU:<~ð©îÃîÃaBýAhhÄßÜbªU£ão+b/<*aÖßÄýãÄð Ü:/.ª|o+aýÐýð/Ð*aß£._ª<r|ÃUzvAZ.ß<ýªOãbvaab/Ö+B b+ýbb@ÜOüuãîzãBýÄzr//hOOÐ+,_|b:rªðUîAU ZÃß¢öýoüoîbÐ/O£åðOßzÄCuª', 0x459AFC3CD4231C669F5738DE1C9C865CA32F74C9FEE294CE0C4D605A4CEC938B67E0DD4047454B3893B6CC6CE23684C50455A81E2F043E63052E3086C3C11AB3C4AD763CBE45145DE33C36471528EBD5C89A56F743745C63A1793B1915C574D19C18A834BF4806BF404B3AF2CEF74DDD425ABD296B23704F51139A450AFB481EC764ABFD7FB6167F9397B6F83A6994AA2B5ED6BA074B6678A859E3ABAEC1B444021B2A6A4B505D8B443D95D46C7815E77FF019AC248D177CFD325526D36AD1D2E271F51E3E2A5EDB004B76C9B6920C2F44FB2CBDA808207504AE6C19FB37B8D96CC2F1FA83F6A954913EABD9164DC66B2525E2D73D132AD9517F1990BD45DFC8929C5DF3A012AC4D52D72C6A5D465EB6E700F3D555E09C589818A803DD18974C5EE2467AF0D3CB1379948565E7BF7C0B2162A8BD520973B200045A8869A8703940DF0B7AFFE4C61435DF5CC05A3671D99B9DE17A6FD9FA546F9479EC7842AAD0244C2508B4020C3D9C948772ADB258CA9381ECCCC7F5C966E5B70A6B6ADA9BF2348C9B0E1169249991A4DEDB0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0xA6BC3CAF58F755DE092E82BE22FCF7A1D2C1F8943D211649B82A5C533A2217EE145D104D2FBB81A49C56836D8B524628A1F0D11CE2BD4985A6F2F62E9B02A3E61794A9C5ECA972174054F8E79EA9BE66E334B14A51FB0B57138AA73CC452099AB29C56F6A9803BF87EB9DB8DC562E7FC33904B665158941B8952B161023553ED4B5D340153C7BD62F8B8CC971F7D92B0A3AB093407A2495A083A6D56CFDAF64866EE41672EE5A96BD3E86E988BFCFB0BC000BE562C1FE5A914DAF9185A66931EC28331313C2A82BD019D601F34B0CE0B3E6296CE1C9F1C4608985253EC90116D5450D507CBCACF957E00558EBEC675630CEEDAF8CFC5022367EA28A3D72ED1718CF3CAE4DBC640749201501B4A8D884122EE47FEB6189D8F4DB2A0E0A22BBD4F2FE302CAFC918846AE6565CE75E6D9D962E681E8E7D2BB73563A9628255FD2674300EC668ABD459166014F50B3CF1A67D9C77EBBAAD5685C288F74C2FB5F204E93FD, 0xFA724EA4A1EF82E5836BBF871F2E4F5EE116BD6D988F3EFB48829606E001BCB543A66C6678D919BA5282D9BDCFEDA2CC10C26D7D3F1DDB6A33535B1F587C0481AE0A4379C2E8E507FD021B16B6812B5BE0CAF51E933F7525CA0133257EFA61D67D570EC5AA290A27DC657F3D02C5B4B227003E8E86E6EE6F941DAEC0B5C08903A044995E7366B0B6C0CE422EBF3823054D80FE7DBBD35096699E308C7A49721BDC7C9CBD2C553FC04F87CEB8A4BFB05BF9A97234C20B47CDF402F5D435243C765B2651429C66D8F7413D2E330597F2A9EDD492D0D63D59D034CEC92A606A8A2B4B575C7E6E6DBF93D07BF384DC14C85C0F4BE661916C9F1DCC810ECE9DAE97D7B203E8AD3B743B8015FFFF5B4AD4F31F066C1A14832999DEB83EB1EC5EF1F621E41CD2D92C1A219B10664B916C44D3F78DEEC730948253886909F4FCC24E212408DD772E021AF3EB4B7F041CDE0772746BBA5F7029C68D61C8496F232524D8C14427898ECCA5F472FBDCE5157E36AA6196809107EEB1FA42501565B9AACFC526A642D7B77EE3A7C6C5ED74, N'ý', N'ZAÃU£aý>uväz:Ð>vå©OOßÃhüOÖb.Cåu©@ü_ðßvðh,<äUüoãoäbo ßãð~îÐbåð~+ ArÜ<O|:¢Öbß*ãåaAðð_bÐåä©büABrAußB@rÜ++üz~_|Öb|ÐAÄaüaAzªh C.u_*.Ðßýv ©u©_üU<r B¢>uý+rbuÜÜðb/vüÐÜUCßðbz.*> .Aßß,ßu<ðý_üu©ªu<,¢ß.~u:Ub>o_Oß£åä£Oüü@UrÜbÐUvîo äãaÐÃîãß*~ zböÐOåB*A/U@å+~£_<uüo*ã©*ßðOöß*¢+rðhßz|*O_håªzh*Bvz Cªoüuv,+üî,rb¢h©::ãuðåÜ¢röbA©|bÃ>A>ÜAüð_ðO+ZoðßüO:î£/b.Uã_Övr*Ã,ßÃÃbh+£<_ZzÐ>>:ZhBãu©/v/ªvð>u                                                                                                                                          ', N'|>ý<:uÖãßß.CoÜ©Ð*b:hC@:Ä+/v/ä/ovîðÐu@aab+UA.vzC£ð@~.Ö>ubAvBAözü,£huuA/u b,Üå>b/bzª>*ãÐ@,ÄÃrUðýCbv¢ÜhBî¢<Zv**Ä*|ýßaÄ ß¢_î¢ß>*Uý,BZÄªävzý.ýab|,O©*Cr.îvÜU¢bä@v,vUZäUbýÐÄÄ@Az*ß|ü>/', N'6e49d5c1-ffa2-4549-a967-17987a1d45c0', -9223372036854775808, NULL, 5595, 128, 1, NULL, CAST(0x0E0A003E AS SmallDateTime), CAST(0.7255 AS Decimal(28, 4)), CAST(0.7304 AS Numeric(28, 4)), NULL, 1.79E+308, 0.1532, 0.6771, N'>åäö/:bB*bCUArÄ+.@@ÖðzÐzzªaÜv*z<_ß_Ã¢åBAî©bÄªÖÃ~b Ðªoä>Oo|£ß/zãaÖ¢Äu£aÐ_.<ävUaÃ_ð/ÃöªÄz©z*_buZ~~ãböýb:ðb~rO©ýUöoäCåaªî,b AðZ<©zhîv.rbÄ>:@¢*.@£Ãb.+rC,~ýö©î£a_¢ªðîbÜÄßÖBoª/<ý_ü_©*ãåB<OÐzZ<@ob<ðåA/@<åüU©Zb@üBa,Ã*ý£ÃÐ>/Ä', N'/*uöBC>ªöC/bß,~uß.©ÜuZbßßbãUo@ýßo._î/*ü,:>bvUZOvB@ÃAb_Uhv.haZv//h¢î,äÃß<:ZzÄuß|>î|h£ßüÜ¢o,¢AröBrb¢hª,ß..Äu|Öu_î:ã£CÐhßªOävu:bÜv@rC£ª©öBBoÄðª,a,¢<OO|ß*bUaüÖC|ö+ööA|Cåzßå_a~.åaÜäå~ýå~ÄÄãA.~Ärý_r*ü£*:ZÖO+zAßÜ©+aOåUýü<+U Äb<Äb©_<,bî*..~ãß>                                                                                                                                                                                                                                                                                     ', N'ðb+A~@ Ð_uåª,AÐrZ:z¢<z+ª:©Ü<£ooýB|.£Ã/h_*ßöOOzö+©+_ r¢uªÃÖ~ðÖäb,bå>ã¢B>ã ÖZ©+r/oÄ,ßZªãÜ:*ßöðäuß>Ð_CZßCÖ~åÄa:*_U< /r+bßÜuCA>ßh|ýCüßðßBo*~¢u.îß Cß|<ª/a./ðÖîOÖªö/zÐå>_¢.,Ob¢Öã,å,ÜðÄv©ü_C_*ýbOüªýÃÜBOðã|ooB.+,©ª_ä@,ãî©Ü OÜör£*rr:¢bbZÄ:ð*_C|_OÐäÐ:r©|ÖbaaÜýoî+@ßåuîÖª_Uhb<<*|rÜýbb:Oo@£Ã|Ab@ÄzAýazÃ ZüUrrb¢BZÐ:ßýaBßî £+~rr~<Ð/Ö/> v@:ÃbßvýBî_.ß~v@ÄrubÃ£ ÖBÜ©Ä:¢', 0x0AB716A52730A4F17BFAAA1BC60AEFFE63A097D3D32BE93A58883F1CE70E1AC5114999DE7EE60D832A7B9824917BCFED4F354F3DF22DDBDFCDDB4D9EF0870A6DDFE50A7C7110807C9214A95792C5A9DCB852CCB1BFDDD2E36EBA10A2996F92A2702B44AC71537C3EB5D3ABDE83834C083270E59EBA59AE0212260DB600DDE2238106E76780FD6A65D68EFCEB32F35FEB6D08702989F4E7167D42994A45C53ED7F0875C7D9BE487340580C82C5DC2D01EC62248F76DB9998D1FD0EA3E63201F105024FAAA97E24F89803C23082BBBB9BB717A83237C466A37EECF5DCBFA48D0E560B7DC38C72587E4D3BF773095906164162FCCE89CFC9409BAC3BCC785838E376B3DA874DF81F44BF39F4FBCCE39A15D6DF9B4CB7CE7496423D27D0369BF5F4F313FC558E8ED84943B80F5FCACD73B81215E5A8BCFC3BB83F443500C04F0E23340008B8F7349747F741F90FBF24CB74B0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 0xE20A65D3806D844164A59D78904A9EC1BBAC4BAB0DD8EADA16AF32C091A78B0475080ED7CE6C7F1D0A32E4CFE2FC8F455C30, 0x00EBEE09208BBDD03AC3B0BD199802873CC7F34F2C2DA1DAF31BF71A20ED64601680048A1904B642AFAFE3478227B54AF90708953D41DED24A13160594A3B04D5F4FA69994786E3810B19F7D4A5958CB66AD2018E4CE23D4A1323F05EEB1AD0CFF95E733801BB58199D0848CA26187F0BB4A9FE7829768902BD2F76238684A5E03927EB472254A4AF79E7F144A973140AA4F4AA312C689672D5607AE2453483085258810E3D27D95FF54EFE418534694A0B7B7E91DA64B3C0816871C1E9C25A3554E8F240E215DD7D1B5A4BDD6A54FD90F730D6C9C5F12DEDA4C08CC4B6177622E4C8EC9C422857F5ADF1768028D6562C7AD0CAA8EB907F63BB04AC219918C703D418E8A178515EDED8E12BC48A9DC4CE6A5049D55CCD47544988D, N'rvüýuZ.B@..Ü*ß:hörB¢B£äÜÖhý©ð.oãßhÐaî_ã|*bböåbßßªßöð@UÃ|.OuBZÖOBU|ýuB/u,*@vAªðî© rbB<U |/OªÐ ÖZªåöÃzB|Äo~||ýü£ö<v_ÖBÃb¢ýU.ßîã>bªU©o©u~~Ö>î@î© ªÜUC:ßýrðß ÃbÃÜß B@üÐz>ZvÃüýªÜ_Äzîßa£ÄAvªbÖ  @@åü©~.,ÄªzvöäÜßaªhvª,oäÜoªbU+Ã_oöÐobaðrC ªz@ÐÃäU/ª:ªbÐh*ar@ü*h*ðÖCaC¢Öý©öovÜä*A:ÖßßÜuöCrªßbß,uÖrU,Ö¢ÃÐ¢©ªÜ©î<*<£Obr@z~Ãßýß@ßÃOZÄ,b@<~.ÖArho/vrÐU ,£ aUå©Ãö¢åußý/>ö@,ßabÜzOzo/U_ßbv<Z_üÃoOäÜ£Üåß,oÄözÃ .  äaãðÄU:.ãßö Ãî_v+uüu', N'/,îaÄzuCÐäoö©öäu:ª uöbäb:ß,bzC/Bîå~/aÄ@hOÄBBOÖUª,hÖ+åOb+_ã_ðîOßª.bAß*.uÐbObuüÖîÄ_ß©o+brÄßuîoOªUaî>©CÄ:vüðaZÃhã.Üöß+B>¢u*ãzüC>b|<öbz:BÜaB_ÃU+Ü>~ýßhu*Uh>ZZzv,>h<ä/ßÖB@ö©oßv|ß,ä| Ðüz*ßuuß_AOÜ>¢ª.r,ðüÖ>:ä<b_ü~Ãå/Öö©@ßCAu~ö_>Üî£r©hªaðª~U@:åÖ©vrähÖA|üB~B*Ä,ßöüb|/*Z/bå>h*.a*uZ_zÖÃ*AåbU~|@©uußZ_+   üÖÜUUåu_buäbobbU¢bÃ*CÄå|uZð|h:üîür*ýîBÜýäÃ> £ªbhÐAî ¢üuvåuAöü©.¢vüböÐb*_ZÄ>.r|ðªUÜßB|£ZaýbB©bßArZåb©_B.©Ã+bð<ßbzz.ÄOãzbªÃbü£ü©<:©zC >Ubh*h.Oª*_oAß+£Cã*ýß:ý:hÜÄaî~UoÄÐßb ~ßÃîåý _ß¢ozß ßöh>AUªý~/,ZAC+:Zazz_A©Bö<©ð o_ÄbU.  ', N'UÖßÐ~UU>ªüro* oý@ßßZA||~Ã£.åÃrÜÄoåbU>~öß.~ @:Öýß,Ð|:ªð_Ãrubðß+ß.r>a<Zð<ðäaå:ubhð©ª>ü¢ Bu**AZ_aýbÜð öÖ@_<U¢~ªAb<obCvräãÜä.ÜhðåO,ãaã©ZÄOOobAÄýÐ<rvååu.ÖÜOðo:_ü,ªªbUÐ~r.O/åöBoãhÜhð~äbýüüBU£Ðhv¢rÜ:/¢ÖýåýãÃ: <£¢*ß+z¢,@|ªCv£*|©bovÐ Ã,@AhAªA*@¢>/¢ZöÜöaßvåß@<*_.o£ZbÐ ðöv O~Ü<rb<ÜrÐvÐZrUðbß~ý|ß>OäÄU|ÖCÃ*©', N'99999999-9999-9999-9999-999999999999', -727874824)
SET IDENTITY_INSERT [#AllTypes] OFF
/****** Object:  Table [#Albums3]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Albums3](
	[albumId] [int] NOT NULL,
	[AlbumName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Albums3__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[albumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Albums3] ([albumId], [AlbumName]) VALUES (1, N'album1')
INSERT [#Albums3] ([albumId], [AlbumName]) VALUES (2, N'album2')
INSERT [#Albums3] ([albumId], [AlbumName]) VALUES (3, N'album3')
INSERT [#Albums3] ([albumId], [AlbumName]) VALUES (4, N'album4')
/****** Object:  Table [#Albums2]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Albums2](
	[albumId] [int] NOT NULL,
	[AlbumName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Albums2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[albumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Albums2] ([albumId], [AlbumName]) VALUES (1, N'album1')
INSERT [#Albums2] ([albumId], [AlbumName]) VALUES (2, N'album2')
INSERT [#Albums2] ([albumId], [AlbumName]) VALUES (3, N'album3')
INSERT [#Albums2] ([albumId], [AlbumName]) VALUES (4, N'album4')
/****** Object:  Table [#Albums]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Albums](
	[albumId] [int] IDENTITY(1,1) NOT NULL,
	[AlbumName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Albums__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[albumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Albums] ON
INSERT [#Albums] ([albumId], [AlbumName]) VALUES (1, N'album1')
INSERT [#Albums] ([albumId], [AlbumName]) VALUES (2, N'album2')
INSERT [#Albums] ([albumId], [AlbumName]) VALUES (3, N'album3')
INSERT [#Albums] ([albumId], [AlbumName]) VALUES (4, N'album4')
SET IDENTITY_INSERT [#Albums] OFF
/****** Object:  Table [#DeepTreeTable]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DeepTreeTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeFlag] [nchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[B_Int] [int] NULL,
	[C_Int] [int] NULL,
	[D_Int] [int] NULL,
	[E_Int] [int] NULL,
	[F_Int] [int] NULL,
	[G_Int] [int] NULL,
	[H_Int] [int] NULL,
	[I_Int] [int] NULL,
	[J_Int] [int] NULL,
	[K_Int] [int] NULL,
	[L_Int] [int] NULL,
	[M_Int] [int] NULL,
	[N_Int] [int] NULL,
 CONSTRAINT [PK_DeepTreeTable__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#DeepTreeTable] ON
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (2, N'B', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (3, N'C', 1, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (4, N'D', 1, 2, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (5, N'E', 2, 25626, 252, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (6, N'F', 24, 26, 345, 156, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (7, N'G', 7098, 798, 7899, 235, 644326, 153, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (8, N'H', 23, 789, 789, 65, 4326, 4, 1, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (9, N'I', 256, 798, 43, 1, 5634, 4, 2, 8, NULL, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (10, N'J', 26, 79, 2975, 25, 435, 2, 3, 1515, 8, NULL, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (11, N'K', 26, 11661, 9753, 425, 1, 2, 5, 8, 8, 8, NULL, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (12, N'L', 165, 789, 30714, 25352, 7890, 34, 4, 8, 8, 8, 2, NULL, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (13, N'M', 563, 78, 17853, 6354, 790, 5423, 5, 8, 2626, 8, 2, 4, NULL)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (14, N'N', 36, 8790, 91753, 52, 32525, 98, 6, 8, 8, 8, 2525, 4, 3)
INSERT [#DeepTreeTable] ([Id], [TypeFlag], [B_Int], [C_Int], [D_Int], [E_Int], [F_Int], [G_Int], [H_Int], [I_Int], [J_Int], [K_Int], [L_Int], [M_Int], [N_Int]) VALUES (15, N'N', 780078, 3245, 7951, 752, 790, 78, 6, 8, 8, 8, 2, 3, 2525626)
SET IDENTITY_INSERT [#DeepTreeTable] OFF
/****** Object:  Table [#DataKey_VarChar50]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_VarChar50](
	[Id] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DataColumn] [xml] NULL,
 CONSTRAINT [PK_DataKey_VarChar50__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_VarChar50] ([Id], [DataColumn]) VALUES (N'', NULL)
INSERT [#DataKey_VarChar50] ([Id], [DataColumn]) VALUES (N'aaaaabbbbbaaaaabbbbbaaaaabbbbbaaaaabbbbbaaaaabbbbb', NULL)
INSERT [#DataKey_VarChar50] ([Id], [DataColumn]) VALUES (N'text2', NULL)
/****** Object:  Table [#DataKey_TinyInt]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_TinyInt](
	[Id] [tinyint] NOT NULL,
	[DataColumn] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DataKey_Timestamp_1__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_TinyInt] ([Id], [DataColumn]) VALUES (0, N'66666666-aaaa-7777-cccc-edededeeded0')
INSERT [#DataKey_TinyInt] ([Id], [DataColumn]) VALUES (1, N'11111111-2222-3333-4444-555555555556')
INSERT [#DataKey_TinyInt] ([Id], [DataColumn]) VALUES (5, N'11111111-2222-3333-4444-555555555557')
INSERT [#DataKey_TinyInt] ([Id], [DataColumn]) VALUES (214, N'66666666-aaaa-7777-cccc-edededeedede')
INSERT [#DataKey_TinyInt] ([Id], [DataColumn]) VALUES (255, N'66666666-aaaa-7777-cccc-edededeededf')
/****** Object:  Table [#DataKey_SmallMoney]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_SmallMoney](
	[Id] [smallmoney] NOT NULL,
	[DataColumn] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_SqlVariant__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_SmallMoney] ([Id], [DataColumn]) VALUES (-214748.3648, N'Testcase')
INSERT [#DataKey_SmallMoney] ([Id], [DataColumn]) VALUES (-1.0000, N'Variation')
INSERT [#DataKey_SmallMoney] ([Id], [DataColumn]) VALUES (0.0000, N'Zero')
INSERT [#DataKey_SmallMoney] ([Id], [DataColumn]) VALUES (4.0000, N'Scenario')
INSERT [#DataKey_SmallMoney] ([Id], [DataColumn]) VALUES (214748.3647, N'Module')
/****** Object:  Table [#DataKey_SmallDateTime]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_SmallDateTime](
	[Id] [smalldatetime] NOT NULL,
	[DataColumn] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_SmallDateTime__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_SmallDateTime] ([Id], [DataColumn]) VALUES (CAST(0x00000000 AS SmallDateTime), N'request   ')
INSERT [#DataKey_SmallDateTime] ([Id], [DataColumn]) VALUES (CAST(0x4E78015B AS SmallDateTime), N'var1      ')
INSERT [#DataKey_SmallDateTime] ([Id], [DataColumn]) VALUES (CAST(0xD6070000 AS SmallDateTime), N'oOoOoOoO  ')
INSERT [#DataKey_SmallDateTime] ([Id], [DataColumn]) VALUES (CAST(0xFFFF0000 AS SmallDateTime), N'response  ')
/****** Object:  Table [#DataKey_Real]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Real](
	[Id] [real] NOT NULL,
	[DataColumn] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_Real__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (-3.4E+38, N'ASP')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (-1.12345684, N'SQL')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (-1, N'REST')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (0, N'LINQ')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (1, N'AJAX')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (5.05, N'EFX')
INSERT [#DataKey_Real] ([Id], [DataColumn]) VALUES (3.4E+38, N'DATA')
/****** Object:  Table [#DataKey_Numeric]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Numeric](
	[Id] [numeric](38, 18) NOT NULL,
	[DataColumn] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_Numeric__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(-9999999999.999999999999999999 AS Numeric(38, 18)), N'small')
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(-1.000000000000000000 AS Numeric(38, 18)), N'ribbit')
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(0.000000000000000000 AS Numeric(38, 18)), N'baa')
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(0.123456789123456789 AS Numeric(38, 18)), N'moo')
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(1.000000000000000000 AS Numeric(38, 18)), N'rawr')
INSERT [#DataKey_Numeric] ([Id], [DataColumn]) VALUES (CAST(9999999999.999999999999999999 AS Numeric(38, 18)), N'big!')
/****** Object:  Table [#DataKey_Money]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Money](
	[Id] [money] NOT NULL,
	[DataColumn] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_Money__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (-922337203685477.5808, N'MinVal    ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (-100.0000, N'Astoria   ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (-1.0000, N'Minus One!')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (0.0000, N'Zero      ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (0.0009, N'asd       ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (1.0000, N'One!      ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (34.5600, N'qwerty    ')
INSERT [#DataKey_Money] ([Id], [DataColumn]) VALUES (922337203685477.5807, N'MaxVal    ')
/****** Object:  Table [#DataKey_GUID]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_GUID](
	[Id] [uniqueidentifier] NOT NULL,
	[DataColumn] [varbinary](max) NULL,
 CONSTRAINT [PK_DataKey_GUID__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_GUID] ([Id], [DataColumn]) VALUES (N'00000000-0000-0000-0000-000000000000', NULL)
INSERT [#DataKey_GUID] ([Id], [DataColumn]) VALUES (N'11111111-2222-3333-4444-555555555555', NULL)
INSERT [#DataKey_GUID] ([Id], [DataColumn]) VALUES (N'66666666-aaaa-7777-cccc-edededeedede', NULL)
/****** Object:  Table [#DataKey_Float]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Float](
	[Id] [float] NOT NULL,
	[DataColumn] [image] NULL,
 CONSTRAINT [PK_DataKey_Float__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (-1.79E+308, 0x4D696E2076616C7565)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (-1.5, NULL)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (-2.23E-308, 0x736D616C6C206E656761746976652076616C7565)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (2.23E-308, 0x736D616C6C20706F6173697476652076616C7565)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (7E-06, NULL)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (3, NULL)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (5.1234567890123461, 0x6869676820707265636973696F6E)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (3.5E+38, NULL)
INSERT [#DataKey_Float] ([Id], [DataColumn]) VALUES (1.79E+308, 0x4D61782076616C7565)
/****** Object:  Table [#DataKey_Decimal]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Decimal](
	[Id] [decimal](16, 2) NOT NULL,
	[DataColumn] [char](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_Decimal__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(-99999999999999.99 AS Decimal(16, 2)), N'oink      ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(-1.00 AS Decimal(16, 2)), N'Neg val   ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(-0.01 AS Decimal(16, 2)), N'Small val ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(0.00 AS Decimal(16, 2)), N'zero val  ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(1.00 AS Decimal(16, 2)), N'pos val   ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(5.33 AS Decimal(16, 2)), N'rand val  ')
INSERT [#DataKey_Decimal] ([Id], [DataColumn]) VALUES (CAST(99999999999999.99 AS Decimal(16, 2)), N'hoot      ')
/****** Object:  Table [#DataKey_DateTime]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_DateTime](
	[Id] [datetime] NOT NULL,
	[DataColumn] [binary](50) NULL,
 CONSTRAINT [PK_DataKey_DateTime__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_DateTime] ([Id], [DataColumn]) VALUES (CAST(0xFFFF2E460152FC77 AS DateTime), NULL)
INSERT [#DataKey_DateTime] ([Id], [DataColumn]) VALUES (CAST(0xFFFFFFFF00000000 AS DateTime), NULL)
INSERT [#DataKey_DateTime] ([Id], [DataColumn]) VALUES (CAST(0x00006FC600000000 AS DateTime), NULL)
INSERT [#DataKey_DateTime] ([Id], [DataColumn]) VALUES (CAST(0x0000901A00000000 AS DateTime), NULL)
INSERT [#DataKey_DateTime] ([Id], [DataColumn]) VALUES (CAST(0x002D247F018B80D4 AS DateTime), NULL)
/****** Object:  Table [#DataKey_Bit]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_Bit](
	[Id] [bit] NOT NULL,
	[DataColumn] [xml] NULL,
 CONSTRAINT [PK_DataKey_Bit__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_Bit] ([Id], [DataColumn]) VALUES (0, N'<Root />')
INSERT [#DataKey_Bit] ([Id], [DataColumn]) VALUES (1, N'<Mapping xmlns="urn:schemas-microsoft-com:windows:storage:mapping:CS" Space="C-S"><EntityContainerMapping StorageEntityContainer="dbo" CdmEntityContainer="northwindContext"><EntitySetMapping Name="Categories" StoreEntitySet="Categories" TypeName="northwind.Categories"><ScalarProperty Name="CategoryID" ColumnName="CategoryID" /><ScalarProperty Name="CategoryName" ColumnName="CategoryName" /><ScalarProperty Name="Description" ColumnName="Description" /><ScalarProperty Name="Picture" ColumnName="Picture" /></EntitySetMapping><EntitySetMapping Name="CustomerDemographics" StoreEntitySet="CustomerDemographics" TypeName="northwind.CustomerDemographics"><ScalarProperty Name="CustomerTypeID" ColumnName="CustomerTypeID" /><ScalarProperty Name="CustomerDesc" ColumnName="CustomerDesc" /></EntitySetMapping></EntityContainerMapping></Mapping>')
/****** Object:  Table [#DataKey_BigInt]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DataKey_BigInt](
	[Id] [bigint] NOT NULL,
	[DataColumn] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_DataKey_BigInt__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (-9223372036854775808, N'ocelot    ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (-2147483648, N'cheetah   ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (-1, N'cat       ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (0, N'tiger     ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (1, N'leopard   ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (5, N'jaguar    ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (32767, N'lion      ')
INSERT [#DataKey_BigInt] ([Id], [DataColumn]) VALUES (9223372036854775807, N'lynx      ')
/****** Object:  Table [#Configs]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Configs](
	[Id] [int] NOT NULL,
	[OS] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[Language] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[Architecture] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__Configs__7E6CC920__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Configs] ([Id], [OS], [Language], [Architecture]) VALUES (1, N'ööb,ßüUªvC+u~ªäu@îä::UÖ<BBöhä_<üo>ÄuZbZUä *BhßðÃÐÐÐ|,ð/ÃOªZÄACÐÄ¢.bbå£ö/ª¢~Cî© Zov,hßð~UbAAÃÜ~uÖª¢:ÐÖãßBßzöÜ* ß@va_ÜÄ~îÄãßAäªU£A.ß>vr~_z:z*>üßîÐu@.ÜA:baÐÖîößð>_£ð+@ß_ Ã:uz,U,ÐAîZ£bãäýüÜ>@:Ã~>_ßßäb/+îÃrÐzßh£Ü aåA_Z.ÄÜð.Ð aäaåÐÜ~/_Ã~@+öåB|OßhÄr:ß|üU,BbCA£~© Ð/b+b ÜzZ>o @ªuÐ~ªªß.rzÜ@O*:||åßv|+CC:b,/h@bßîå<ä,:Aýåð@hZãäüzÐÐª+büUB¢h@r+v©Uü¢zåªOCäß,£ãýßªr.C.ß:/ãðuÖÖz>Ü>£ãå r_BÃ_ªÖ|©z,Cß@ ßvãåÜ_ß,£ä.ß*AåýýB>.© .äOÄäoªã¢üßOýUßüBß.o/ãCß,_Ü ,azC£:ãýÃb©£ßßu:aOA£OA', N'ý>î~Ð+ z+Z/¢£Ã@_,ßvACÜ*ªª©ßr@hh+aªCo_vU Ö*v@+ªß £>U*Öý|uhZüb|CßUhbªb|+ýh~övÜ>ãuaä<|Aåß¢AAåavzOÜ¢_A,zýÖÖZ>CÜßÄßÐzbörb|Ü* :~aÃ*Ðß.O¢u::ðaCÐv@ÜAß UhÄr*üÃbÜÃbä,ÐbðÜZ*¢b|vãhðB Zv|z£vÜ_£@<:<|:Ü,b¢+© Ð<©OU©åäb.ä+ß@CÖuhö@vUßîo>C>:BzÐ©ö@b¢:aC©ß<b~BÜö£¢ã<.hb<©ÖAÐ_+,üBhÄãýbhÜCãä<ÜÜzbÐOãßUßz@åA.ä.v|Z,@ö:,öö~>b+~v~Äî/ýa+@|ÄvãOöö£v*ãv~bUßä£ßuv O:ÄUzz>./vOÖßß©¢uýýöUÐ_zÃ>', N'~*rovU_ßCð¢£hãð|BãU/hU.ßuzb:ð,|ßã.ãBbA<~vä')
INSERT [#Configs] ([Id], [OS], [Language], [Architecture]) VALUES (2, N'+*ß,ßüuãvAö*ð,ðüObîÄ+ßßÄaã>b+©oZÖh/ä>Ã£öOÄ_ß<>o©B>.röÜî__B,ü:o<~å£ªÜöb<</ö_£ZaU@©ßýävü üuîz,Z©<AUöðãå,ÐAAðÖaUUU_ääz£oªzîz¢_bÖbrÖ:>. B,¢ZBüã*r Ðý£', NULL, N'+ãä££a@vã©vöÄý_Ðãð::UA_rÖüÄüî|,ü|Ü,ª¢ÜÃbå+/.Ãî¢rßýhÐ bO Z£å©z_*vvz,öªðCbu+~ä~C@ã >Oð.©£<~b ýåaÜ*aoo,@ÃªäåB>ö>zÖOCb*ß*/äª£î*_ªhou~uä.ühz£å b OÃýZzhäã©üý~z_Ð¢ bUÐðZ+v: rýÐÃðA,@/üðbU:Zz_¢UvC/.Ãzßbz.å ÄßýöåCüAörßCÜÄUßß:ªÐ')
/****** Object:  Table [#Colleges]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Colleges](
	[CollegeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[City] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[State] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Type] [nchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Colleges__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[CollegeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Colleges] ON
INSERT [#Colleges] ([CollegeId], [Name], [City], [State], [Type]) VALUES (1, N'University of Washington', N'Seattle', N'WA', N'U')
INSERT [#Colleges] ([CollegeId], [Name], [City], [State], [Type]) VALUES (2, N'Seattle Community College', N'Seattle', N'WA', N'C')
SET IDENTITY_INSERT [#Colleges] OFF
/****** Object:  Table [#FailureTypes]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#FailureTypes](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__FailureTypes__1B0907CE__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#FailureTypes] ([Id], [Name]) VALUES (1, NULL)
INSERT [#FailureTypes] ([Id], [Name]) VALUES (2, N'£öhãAßýßßÄ~.ÜzbÖhB¢ßAß+<ßö.ªöå|Aýß©zªZÐªzrbbßbä ã_ÐÄ__ßß|©ab.+äðo_/ý¢Ühã*a>ãÖz£ÖBbÜ,hä>/ýäÐöv©rö hîZ¢äÃb.î*@AöbUAvý ~:ÜzbýaÜAr|Ãß|@ª,+Cu+ÄýU>ý.AÜ<ða©ä*ä+ªu¢äðAu ©Ã:/:ä£hÖüä<_ßb~üa_Z|aür£Ußh>*:ÃÜðãZö~z¢väîî>,ßßOAåî<öÜ£ÐUßrb@£,>OÄªüÖ£+ÃîaÃBbÄv.ýü::|rãor|ÃßB~Ðu@, ©uã.,@u~ðb_©__ßb|rööüå:Ürö~~ v<~©,~rh_bö ¢îb£îovz>OA|:*ßÖbr/ö¢U£ªöýhü~öahýª ü+CzßzZãB:auAÜ:b ¢aÄ,v<ÄîaZÐ|Oå~ãAã+üî¢¢ .|hðÜBOhå©@ýåöÄå,,ßa|Zð|/+ýöbB/ý++üÃ ÜhîC~©>+~üBC<|@ã')
INSERT [#FailureTypes] ([Id], [Name]) VALUES (3, N'h£~<.BubßU@¢>üa|¢Uåîüª.Oä,îß~@îhÜªãbÖZÐßå_äUäü:hCðövüÄß@u.¢ar,:v*,ãîhüß @ba/ÐO åöÖÄ©~¢ObÜzß £Ð~>bªå<C_aoC¢ÖåÃ,ßABÖb/ÃßÃÄrÜCãh<©Ü>@*hZãÖ,Ö|_üåz+,>ÜUîÃoßZðOv .oA£öý,OA£ÖUý£©hUÄãî.Ð *uÖB ÄbuC~/ý~@åaîðr*äAýîA<©Ar*+ðÃã©/råb:ü/¢£|©bª+Äöür+:zuãbäAAAÜu>zÖä:ßaåÐ,.. *+,b©<,,Äz+*r*ßO.ð.ä/b* ÐîåðB:übîå>Ã<ü+_+ßßý+>v<rÃ <a_åZab Ö~<zU* :zãCÃ/_å<¢v+üU¢.Ð,h>A*ýuÃ/£ªöî@_£bbü¢baåðhbva¢BbhOÖîCãZBz¢/vßöö~CÐÐðr©ß£,U|îZ')
/****** Object:  Table [#Failures]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Failures](
	[Id] [int] NOT NULL,
	[TestId] [int] NOT NULL,
	[Baseline] [bit] NULL,
	[TypeId] [int] NULL,
	[TestCase] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[Variation] [int] NULL,
	[Comment] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[Changed] [datetime] NOT NULL,
 CONSTRAINT [PK__Failures__1CF15040__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (1, 1242201900, 0, NULL, N'ðröðUvrO,îoýAb<:ã@u_ãobª.Ü~Z<b< Ð@äã+, bA/©ðrªCÄÖ©.b,Z|ÜC >:©îÄz/ßv>O¢B:rh@.>h.ß£å_ß£/ð:üBÄbåä,CÐ*>+*¢@C~üävÜovüðÃßß*î©aîß@bðb| Ð,©h£A:A*.uÃ~©©ßßÖCrbªÖãÃ><_B/r_,@@+<h åUß/öÃü/uhÄvOð+Zh¢ã:h|îððr©zboz Ã.uüvoßhÄb', 1, NULL, CAST(0x0000901A0000012C AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (2, 2147483647, 1, NULL, NULL, -2147483648, N'îä££ãÄðð£ö:.ãö~@u/ýr ªhbOýßa*.+ã+hîOOäzýuð£.ªzzîý>ýöZ_<zÄ~<äîî|u|¢bÄb. *Cýü,öÐ:ä©îAä//|ß£~:ª,/Bz+/ãýv~ã|O¢ßZ|b*üC¢OýåÄÜhbub:üÐü©î£A@î<~bb:©ßªvv~U<<ÖvB/©£ÄðA_+ª:©h,îbãb£ACö/Aü@ßü:u>Ur.ßu*äO_|_©ãuh<å*_.,ßÐhoý£©rhbboCr<Ä|ÃÃüåC<|v~ßãAîªðßB.:v©uÄrý,<:@ÐA©Öz©ã:>Äv~å.ãßoU:h*aßa/î:ýÖîÄýBÖãÜOZ~öb+~ÃrÃÜßä+ãoAzoÐU£z.räÃªýv<h|b£ä£>bý.BßÖh£C,b|üÜ~C@@üOÖu©ýÄöu¢UÜÄü>@vOzhã|v|ÖZ_/zÃ|:_hî<Öãz/,ä_>, ©Bb/@,ß+©,îO:+/v¢@bßU~bAÜ.', CAST(0x0007CB5B010FF9C8 AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (3, -77590511, NULL, NULL, N'¢:,äoo¢o:>||+ðaCua.ªBBAa©C>ö¢Ã*habÖüäÜ*ðobC|<rAöã¢ÖA*| _/,rb£©_Üh/| obv:£>/aü:_<b_Cýö Ü:Äu@<|zv<@*.A£ÄÜß+Z+äCz~üäÐüB||ßß©B¢ö_Üî Uü>ªb>ß+Ü ª©öî*,£_¢ã oîü@~+_bãU~,b¢hßüð*îª UöoäÖÖuzýobaBb_b:CZZðbavîZr~©¢vðÄß©a+Ð¢Üä@v/ A', 189969080, NULL, CAST(0x000827B6006C2B68 AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (4, 1204469754, NULL, NULL, N'u¢@a++¢|ßZÐZ*@ßC_Ð* rb.ð_:/ðov>Büuªß:AhåÜb<bvå~+öaîãz©¢hr~<ðöÃ,|Ü£zCðü£>~<_h~ ©©åbýo*©ßzª:¢å|uöb©ðzª~:/ü+.UãC<_äðßbäÄUrCäý@:*Üð+@::©Abb.¢aßö.öOCðÖöÖob+.ÄOÖ~öý¢~<åz©aåãäÐ¢UÃ:vÄîß,_ªßor:BböãZ<<£,r:@£@BbîBA_ã©ü/<äÜ_:aÜ ©ä|üðý~Uh~zvßOUZZ<ÃÐh©ª/b>+/ÄöO,.CãßühCî>ß,U+Ä¢Zª>UüzbraUhî.¢h_r.îãÃ / o+ßu Z*b*åbãrÃub+ðã|ßUÃv+Urã:åðüoOBª@ãäÐîb__|z >Oã<Ö<å>ð©@äbb>ª<öB©A~¢|@bC©ÐÃhÄbðÃ£¢,O<vÃOðaýÜ£,<bª<£zî|vöÃ*uÖu@ßßo,äübÜ¢,*.Ã|_z<', 2147483647, NULL, CAST(0x001B25CD006607EC AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (5, 1807599313, 1, 1, N',<Cö+a~_rð:äbz>@UBuoh>ªo+uOðýîAAý£ð|¢b©AãÄ_¢~ÖaAZ,ZÐüb.îäü|oß£|zå/ä.+ðîO.r /üý~ªro£~¢ZbðuAÄ£>@rOouÐhîª£+<BzÜÖýðå,üö£aüÃª+ðO*uvÃîýhZÐU/Äª@ZC@üZOUBßv@||oO_hÖü.zhu,.ÜªÃr¢öv*hÐäo¢BÖðU,ýO*/ýBOC©ÐOrhåC|@||îrzvr.. +>©r>bÐuub ª~a@v@oöªüAb~O+*r©ðha~Üß¢a@Ãh,£î>OüüåÃbÐb<ß* .BACÄ__zðzAßrîva<ßrzh~.u.', 2147483647, N'ßar>aBv:£<ã¢,uã_å_¢@Cr/Ã_hß.o_Ö>Ü*/©CÖß_~©ãü£©£/', CAST(0x002D247F018B81FF AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (6, 1742022022, 1, 1, N'~>üäzãzCÐ äbßo/hÄBð/*<bå<*ðÜzzÃö+ãBªßüUÖ*ÐUzßª¢ öBÐÄå_UC:b,öî@ß.¢î©übã ÖãOb¢uÐBäÐªãu|übßªühÐ|zããðß©åð£vBÖýåÃ:ªåÄý@rz©~ä©ãä+BÃªr_*UÃ.bröb*Ð<~B,*ýB.ÖbB_o*üabbZo,hß**uî<©Ü~hüöBZ~/>,aZäh/ßb*:boäßO  ßÐbhb¢Öü:ü~uU©/ª~åhCv£ð_h|a¢ªÄ ÜªÜ<ã,*z,_åÃ:uäBÜª_Zîbuboß>ý>ªC+Ð£ß¢<rªuu~Ö~,ö:zZ<Uab/.Ð+Ä aå*ßÜ.@öß.Uª~a b Z£©', -779570188, N'Arår+ý**OuaÐr£Äªß,åöðv BãßU©rZî£Ü/~_¢Ð:ýªä©@Ã_oaü©~ ªßAÖåß+_Ö©hîz*öÖboüböî*bªß,z£uÃ_Ä|bý.ã©ã+îuðÄÜr*~.ößÄ>,Ã:ühb,bî@ýªOÖzü u>CÄA©.*ã~bÄUvîãro+@/Ã~:zaÄöB~:A¢bÐZ~Ü+Ab¢z<* £UbÜü~ö/î_ð©.aü:UvCaBo_ªU/ß :ß+BBü+:ä ªåBOOr+ð+*bª,b* U<Ä<__ÜªüBoß|/£@~ð£Uðîý uzb/Zä. ÐÜÐåÐ,ÜbB>rbA_*ðÄ@A<,C. >+vu<*zCÃãBÜÄÖßrh~h.ÃîZÃühÄÖãööª+,+*h|~ÖOB,r:|uöÐã ßýý£ªb|v ¢|vÖå<.z+/b~ý+U~>BU,îÖaaªh¢+ÖÜv|Zvh¢ã£züoð|zÜÖß¢ÖbBý å.ä_vhÖäb|ªo>ZÐUªÖäÜvuýOÃUä aªÄÃZu.ãvb|@|CB+üov:b+oZUub£.ÃãAv/ã/ãðåväåý@r uoh¢u_ü,v¢:./bOÖ/~u_Uhv£@', CAST(0x002D247F018B81FF AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (7, -415652550, 1, NULL, N'îaî@Ü:ZbåOuý/<ßBB:.z£Bbî<CovýßbUã @bZU£Cö>+üb', -185486228, N'.ÐÄ|*AßOÖBßAÄåîbAüÜv©ý_üZ|Ð~ÖbðÄ@.Äß|O+ o£CUa+OªrßÜ,äãh.OÖZÜzu/ÄOB>zoB@äª:BAåZüð,h+ðv_äÃvZ¢ßîªAör,bîäå/CZðß.ÐC_åÜ++oã>:¢UßAå:Äöa/oß<ubb@Äv¢ Ð/ühÖÖA_uäCÃð/©bã|©~ýßý|zb©Ðzã:~åhåßãåCÄÃ.Ü|bC_Ü+ýÃoªb*ö:,ãÃbZu,.@ßß_åuö/öå|a.@O>î+zozüãßö /A/¢.O,h@zÖu>ÜÐÄ£oÜðAå©OOððUC|ýaª_ßða <,¢ZaåÖ©ä_.~ Ãaaz|+U:<voAü ÐªüýßÄ ©,zÖäÜ/>b,ß_uZýªUU_Öäýh<vb|abî|ªäaa¢Ã_Ã~ßö/å <ÐAÖaÐÖZü:©Uã>ßu./oãu©uv+îÃ_ööî|vhÜz_vboßzî|*+äbä+,@vbCß>Ö+BAr A_BýÃÐ>', CAST(0x0000901A0000012C AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (8, 48979093, 0, NULL, N'ðýA£©|_,ZuaüßÜåüCªýåaU,oz+ÃuhvauAãã,b,@Ö<oßÜÃÖÃîv.å.ðî£©ªä zåzUArßðüö>åÐbu,ä/¢<_©ßüÐ,ªzhOoßrbråoîÜ|åãÐüÜ@h Ã>ßUü+Ü£B*@¢ª>ßÜßÄÃ©ä Uö£bªÄðßOrvvª¢,îßßZ|ßß,oü+Aüobã::*vä~ðÖbÄvOz+ îo+ååä,z_/|>bUÃü£C+ßßv+_zîbuä_¢bß£<ä~ZOüboßßzBã@+Ðªbö|_£ßã|ýßo:ýß<Ä/Öß,+ð.Ä¢/_Uub@ãCö|huCãbÐÄroîÄßaß,,Ü|*uC_äîrUª*ðZ  bU*< ðÐ££Ðvßð¢_.ß©ÖZÜÄ/ª*r£ßh+vä~,ÜÖ<îbäãÃ|+vbý~', 1, NULL, CAST(0x002D247F018B81FF AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (9, -2147483648, 1, 1, N'ýýbÃÜz+ääö¢*ªö<>Ü£+<.<u@ov:,©uzî+ahî_h/Üüä/Oü_~£:î@+zhî@Ü|_>äýr¢£Äýªß~ü¢r~>_Ab_ªÃßöÖÃö~uBªCüªöðý£ß@U|üãäðåoÐ¢+<Ðaðßbb:ðÃCÖ>O:aAv£@îu@zbÄýBßßÜä~Ðh,ö©håöråßCß:£,ðäîBC<vrÄå', 2147483647, NULL, CAST(0x0002EB5D01175628 AS DateTime))
INSERT [#Failures] ([Id], [TestId], [Baseline], [TypeId], [TestCase], [Variation], [Comment], [Changed]) VALUES (10, 2147483647, NULL, NULL, N'¢¢C£zªüå|<î@åz*rÖã©*ö>*ßZ+Ü©_¢B©bãäÄ ,,vÜC£Ð© *ß>ªß,|ªuCvz*öãz*ð+u:b/¢ä*/.~ãã¢hüaüoðßÄC:¢Ð/a', NULL, NULL, CAST(0x000F87E101213968 AS DateTime))
/****** Object:  Table [#LabOwners]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#LabOwners](
	[ownerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FirstName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Changed] [timestamp] NOT NULL,
 CONSTRAINT [PK_LabOwners__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ownerAlias] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#LabOwners] ([ownerAlias], [FirstName], [LastName]) VALUES (N'christro', N'Chris', N'Robinson')
INSERT [#LabOwners] ([ownerAlias], [FirstName], [LastName]) VALUES (N'davebarn', N'Dave', N'Barneby')
INSERT [#LabOwners] ([ownerAlias], [FirstName], [LastName]) VALUES (N'steveob', N'Steve', N'O''Brian')
/****** Object:  Table [#LabIssues]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#LabIssues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueType] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_LabIssues__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  Table [#FailureDetails]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#FailureDetails](
	[FailureId] [int] NOT NULL,
	[Log] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__FailureDetails__1ED998B2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[FailureId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (1, N'A¢AU+Ö£©Oz å@ÐA£+bã+|ÖÃ,aýßÃÐb >@ÐObZßÜ|ÃAuð£,¢bCbÄo~Aªvð¢u,ðöðCýã*Ü|ü+_zvÃ__ÜÖ¢*Ð~©+Oð>hî')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (2, N'ã@oÃoãz*bÐhä~Ar+v')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (3, N'.oöåü:|£¢+îCZv,@ÄÐ<>..ß~öäÖBZÜOa ,.>b<ÖOuuåå_rÖvÃÖrÄÖCîa**öBÃ:*äråßîäå:uüzZr@ðbÐC>._~<vr<.hÃ@©<ÖäAÃ£.ä.ßZüÐ:ßÃ<.zrýrbßZÐ¢~./ÖÜÃzã@ <U, UbªU¢Ðß+ÜðÐ, ©ZÄbÖüAß:üb@v.ªz+ýOð')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (4, N'¢Üb*:<Cbðö,rOªv@>ýßãUö>åÄ,+åö>ªü~,@ªÜ¢¢Z>*/:Z+*ÃuÐÖ£Ä,o©Zr¢bCUB.*.ªßB©Öå+_*î@A~*bÃ©uÖüvruãz|ý|OvvaCåÖ_ª_ýåvOAvoU¢va.|å©,o.~U*råß*b¢@ðÃª>BCuäÃboöÃbðÄ_ÖÃ,*ÐÄ ü*rßý:ãö¢£ ð©@ :£ßÃBv¢|Zvov~,UorýZ@Ðî/äuUr bÖabA/UÖ_A¢b')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (5, N'B_Ö.<rð>~.äC~~äðo©ßÜÐ~ÃªArãÃßÃvAîCãÜv,ÐÃ >å+*r|a_zUÜÐã:@äo+Z*UÄåoãr:Ðb£ÜöB:uÃzvã£ãh>:<Zb© ðCa:')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (6, N'CzzubUÜä~ äÜüOäa£îAå£aÐ*B/ÐAh£Ã£Ü.zu¢~ aãÐ.ýu|z:ßOÄZÃoÄB|üh£>ÐÄö£uý|å£+b~AAðªÐh_CvUöð.ü¢Ãr£©:Ã>Ba:*a|Ö~hß©Ä£Cß>ãßª@u>ÖauU*å©aÃªå_öîh+hü+ÃBhvÄz@åÄßääovªv:CÖuåðhCbßCoå*:©£o_.ªÐ@ößýürãÐÄv*£Ãv,Ð>')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (7, N'Öý /ßðaA©zabbCýoBöÜßäßZ+oÐCãßA~ßbzzU@îÄhZuüßð/Zãb@ý¢Ãb¢öÃüZBßor£ÄrýÖßÐ¢rU£_¢ß</Ð/ZÜzAüha©+||¢¢ Ca BÄ:@b åß,b_aCr@_£@+îªÐbýB.ob~ªz.¢<+ÐZðu/.U~B:©U*bbýßÖCÐ*£:/¢OßrÜÐh,¢ßÜvÄZð<*oü@.oßA@¢>£@ÖAbß>~vªbß*zZ¢@vßv@ã¢,O©ða:+ßÃhU>ååßÃ:uÄ£äZ+¢uh_ZÖÄ+üßå*ã.å<Cîß+£')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (8, N'@b_ ßÐhCvî@Z:,u@ãÜöð~î/Bhb zubCüãa _ã:üÐÜö~ðoZðåß:ÐðUª©¢ª:OÐ.ßårß|/:b©Ã,ßrh¢z@ðäh/<Ü OÄu*Äö,vuzöb+Cªã:ßoÜ CA:A b_z~ü CåªãrB*ÐZCªåO~ö')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (9, N'voÜbbb*hÜ~Ã ÃOUU*ÃåöåÜ<vårußbßü|Öîä£Ã~ å@Öz_Üð@_£ßÄvÖOA>Ã@Oäã|böªh~Ü ª|Ö+a.C:ö>ö©ÃhuBÄß|å*öO.:böÃ*ð£ ÖubvüîÐÖîAÖAurh.ãäåzaåü£,*îü</Caã¢ð<Bühßbßý roOvî_ã>b  äU,åªýÜZ<ðÖOªv©zåÐburî£:uO£bðªªü@ZbuÃb@ßz@>,/ý|ýuuöÜüBCöªö£/ Ä+åzß©zr:ªßrUîÃ,UaUª©ãÄ£ãý.OCªÖ.b>ªo¢B£zUßÖªh|ÄO/_*ð.ã£ä£u:ßUhbãä,CÖz_Öo*ð*åbB@ZZuÖ|UßaUãÖ>.¢äbCÄðÖÐ:üåîßr~<<ÜÜ_ÄO@A,>~A+äî_A*_>äUoo*Üãz äÜbU_ÜbüöÃ/ AÄu Ä+~b*ä/£bªuªªOö*rÃ_Ä_Üãbß|b£Coß@öã£,UUýý*,oð|<*ÖÖbh O>ßð@>UÜßÜ_ðßÃ£Ü__CöÄ_+h¢î@*rzåAA<*:AÄåÖýur|ãUh')
INSERT [#FailureDetails] ([FailureId], [Log]) VALUES (10, N'*/äz,åÖ<üoA+Z£o,öåh/C©Ca/ÃOã üýCBBZÄCÃ|ZO Öö/U:ßä£@_  ¢© äåÃÃîhýOîZO*:vUzäUÜðo|Ou+ýÃ:/ÜÖ<ª+Ußä~ZrÖÜ@öv/,+vU<bãÖOZÄ:zî/_Ðh@ÄÐbA:/vü|o+buüöª OOzÃ¢äröB.Z+Öub,/|Ãz*ßßuBZ~ßU<>+zC*_ããbååb*Aüªßö ©ß,UZý++B~*Z,Äªîðb£ä>_ª<ß/v:ÜU><¢<~ÃChAhýOÖvÐ~<+Öhb_oBãCrî.ãÄîªãbå©ðhðßÜA>ä ß*ÖUüîÄ£Oüz_¢:>rÄO.b|îOäð_uß*v>£u@zãooåßrö©C|CZ©/~bäBÜÖîoå .©åzî:©zCh£A|zªåªubzßU|,Ör£|/Ö.ÃZã:hz£Ö£îßO@Ü|>|<îh©B~_')
/****** Object:  Table [#DeploymentScenarios]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#DeploymentScenarios](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[ProjectId] [int] NULL,
 CONSTRAINT [PK__DeploymentScenar__33D4B598__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#DeploymentScenarios] ([Id], [Name], [ProjectId]) VALUES (1, N'ýU@£Aªzh|/>:vZßbbh,îo£rAuöä¢hÄ|ã>ü¢îu| OÜr|öO+ö|bb~,©öa*b</BÐÃaäå©üz£åä@ßuhöBUäãýz|', 1)
/****** Object:  Table [#Builds]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Builds](
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Builds__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Builds] ([BuildId], [Name], [LabOwnerAlias]) VALUES (1, N'Astoria', N'davebarn')
INSERT [#Builds] ([BuildId], [Name], [LabOwnerAlias]) VALUES (2, N'DataSet', N'christro')
INSERT [#Builds] ([BuildId], [Name], [LabOwnerAlias]) VALUES (3, N'Msxml', N'steveob')
/****** Object:  Table [#BugsProjectTracking]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#BugsProjectTracking](
	[Id] [int] NOT NULL,
	[Number] [int] IDENTITY(1,1) NOT NULL,
	[FailureId] [int] NULL,
	[AssignedToId] [int] NOT NULL,
	[ResolvedById] [int] NULL,
	[Comment] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__BugsProjectTrack__2A4B4B5E__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#BugsProjectTracking] ON
INSERT [#BugsProjectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (1, 1, NULL, 5, 8, N'raZÐuý_O@ö<~_<:îBvrßC©ü*ð£CÜ<ÄabABä@Ü££ðãÖ~åbÄ_ ßý£o~>:_böß~äÜÃßð/_:¢ðh/¢vý*bbZuBßü>bä<_¢z:ãäbbbbÖü¢oBÃäär@z@B:ü©~<bßOÖbüaß :z||rîZzC£baBC£ä OA@ã>OZ~avåaî©ov+,_£üBªßoäa@<üÃ_îÐßýZ<ühZýr¢,zuýîÐÐÄBu+b.ÄÃ£oÖBA~*Coäoüî/.*üåü/ã¢aªÐUü:Ã+Ä|~Ü ü@åObArbCÐÜßvÃä,O:vãaoh+ßªî.Üoª+CB/ßBÄð.£_ååü.ÃÖßý@C özß,@+/hÜ')
SET IDENTITY_INSERT [#BugsProjectTracking] OFF
/****** Object:  Table [#BugsDefectTracking]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#BugsDefectTracking](
	[Id] [int] NOT NULL,
	[Number] [int] IDENTITY(1,1) NOT NULL,
	[FailureId] [int] NULL,
	[AssignedToId] [int] NOT NULL,
	[ResolvedById] [int] NULL,
	[Comment] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__BugsDefectTracki__25869641__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#BugsDefectTracking] ON
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (1, 1, NULL, 8, 2, N'|<Öh¢/ßÖö£ý£ÐBýr<hZîOå:+B>äv,ß|/+ðU:O/>zðoUaßa©o~Äß~ðho©A,bC|_*hB|vßã_AA£üýa£/~BãÃÃ©üü@UÖ|/Ü,+Ð,ßBðUÐäzbÃßööooA~ßöZäãÃOCAAbZBU¢vU~r_@ *ü+£ýoA~bª@ãý:Bä|CðßrÄßAö:O©Ã~ö>b<~ >rÄ')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (2, 2, NULL, 1, 1, N'/ÖUvOO+AbªÄh¢o*Ä@ý*å<£AÜ>äÜ+*.ðÐÐÖ+ZÃra> _åå+ üoo<OaÐ~<ÜÄ<Öz.ý©©Ãü,äÜbA üä_ðÜo.OîªuåUîaoZ~|r:Ãb*årr,öB,B:ãbäA_a|b|.ßÖß.ßüU~O>ßåZÜrb|öAUßrh.,ähCÖ~aoåZv~Bß,.Ã>rª£üÜBã<:ªA ªª¢+AöãCßª<£ýý>¢üÜ|ª_za_Ü.bAZvh,h_©©üÄÖ:|~©ªCýýÜ~u@©b.ßz~Bªªäßã|<B@/ ã£UaBAü¢,.Ü/z/bÐ:*ÄÖ*båBh~ÖÐCha:_/Ö|v©ö+ãÃr©ãO_+ÜßaÖßÃüÖÜhå@Zu>v.zîå©>:ß, ð©årvüb,üZCv©ã:aÜhÜ/BªOðv@Z©Ãv*u*:Özå~äî:åvuoh£b.©Ä aüUz<ööÖÃ+ÃÃðåîrBoCrväür*üÜCäuvA|Au,u:ßð¢CuBr|ZvU:üv_@bhýAü@Oßå:C©hî©>ü£ r|h£åaß©>ä.va<Aöb,r>zÐßuÜü_/+ªÖ> ð*üð>CAOãÃö<rCOBü~BÜ')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (3, 3, NULL, 2, 1, N'ðåî:*zåCðöbCba~Zoz<bäÖ|ä* .A£ÄOðîra¢r|ß_ßã_vAbaöýßªã_,åozbBaåã:rÜouäaüB~>öUAuzßZz:bãC +> äzo<ß£ü+ >åãoöaÄa ªoäöýußÃ/ß¢B/ÜÐ_o£ðCÄZZ£åßö:o@ACBaÜ :ð ð©BuOÐUbBBªß*Üî¢zåubov _îÖ>a.a.Bu a/ªbBî£Öã+|ÐÃ O,:/Uäo£*|z/Uu£BÃ@|AÜ<AuÜz../üüoboãA_üä+O*BAÖ©©.CÖ©bO£|Ä*u©ð:Ðu+~h*b,ª_Ð£Oã/+ý<C£ýZ©Zðý.ªC©zzðð/CÄ:ªz~A>rö,ýßC +Ö_Z:/|b©oö<vöu/Ðv,z£@äßbüß:*_ýª<ßZ|Ö¢<ý|£ß:*CÖ|ý@£Ð.ß©ãîÐU>£BÐCo*ý<£©©Bß.ÖaÐ,')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (4, 4, NULL, 5, 10, N'<ßåvO©C/£a,ü*<î©zaCZîzã|öab©C>ZZZ.ªðUåoz*ra£ÄßbZBbB/ãîzOü  O¢: oÄBÃðäý..ãßöÄZOãhðÜ|A,ä:+B/öð|ýÃååäöªCh.C©Ã~AüªåÜ<rUãîZÖ')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (5, 5, NULL, 9, 7, N'/b<ãäBbß/UuaÜ+/+Äv..<_o/+Ðãü~CBüß übOãÐððÜ:_vO:<:îA~uî îåA<hvbUzoUO:.,rä_>>Äb¢OOv+@©b>Öðýboh+~©ã_h+aaabbÄOAÐ.¢ßvãZåå||U£>v:*ub>üß£oöUOOUz/üî<A>+UîZbÄÜZýîÄ>Ð_Ää£ÃbîÖ<åa©C¢~C~*O<ÖBCzÜ.Ub+ob_©/Ã~~ u.hÖî*å¢üBro,__Aü:ª_åvßä+zß<ýÃðý@vCÜ*aOÖ/o:oãuÖ>a£:Uýb/ö~v+ª@,¢vOr¢h.ÖÃüZãBÐß©ao_ <*uÃý<ßOrb£~Ub@Oü£~¢bUö>öoöÄhîvrbrîvb<rozU|¢ Ãªbr¢Ã*~ðvU.ÐÐã_CAZß/@AhUC</.zßÃzov|å>>rðªhð©ä Ö+¢rrvãåO|ðü>rîOª/|B|<Z£ª_båß,B<+ßÐbßBª.<Zbaü_ýaa/@>')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (6, 6, NULL, 2, 8, N'Büª©rübÜ::~Ä<ã+>ßoªZ,vÃb|bå+äª+ªA:ýß>öu@rz_@üZ@obA©öo<åßZ_BCbÃðãbü©aã|aäöBbBÐOBZ_Äzãð./||O@üãB/@+Ã/ýª> @aßab.ßBC/ß|+,Bð©_B:©AAaã£|ÐAÐC©b,ßßC.+')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (7, 7, NULL, 2, 8, N'@a_bhvÜÖ:>b£<ýßo.ãbb+zo@ý~Zßvoö©~¢>ýz~/:vý©oýÐzãzUözÄAAuäab_ýü>Z|,bä<ßäã.ªß,£a_Uv:>ar£ü_ªZhZ>å<Z<+ªª©:+@+CåBU¢|ä<ãv_:<Bî.@UO£~ _h>îv,ð|ßbü£bÖäzüÄZß©ªðßýZÃOorOäßß*üÐÄz:_v::>ãao|rZ:+Ü©ðu©îýÄÖ.ªîCãZ,î~U.ãU,o< o')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (8, 8, NULL, 3, 6, N'ðÄ>ah/ÖbßzÐßäã|+ýÐÖ©¢AA_@vÖÐ ÖÄoh<ÃO¢ãoüo@/CB:@zîüO_Üö åÃü<ä©aÄbª|£,o+Öo~Ü>B~ð/bÐ:ã+~O,roß>£*zzoß,>@ßhh_raüa/uZÖ@.CrO </O:U*åBßZ<*Zr.ðvªzZO|r+Öð>ÐhðOBý<ðb *îÜ£BrÃ_bðB ªh>îåoîußÃzähüåB++ßv ªîÃ@¢B+@ã©ý,._ß©rBzaä>©~ð@C ~oÄCãýýuß+Ub_Ãýaz:Ð/B.Ov>v<BuCªCübð©üå|._|äãäU<o@©v<ÜÃUOý/ª|@îaÄÜCB>ÜzA/å å@_*,vÜ ||ãh*ZßÐÃ@výî¢|obÖhü+u¢ã~:ßÄ££A*ÜO*/ä£ÜßÃA¢>oå/vü+v£Cßö ðý_î<BBA<îuÄZCZ/ü Z*ªC©Ãa@Ðv++UüUa>åBå ~ö~Av©ÐýãbãÃÜ@ªÐß ãåü+ß©C©U:v¢. BaÄ,aB.AZ_îr@Ã::£ZÃ|Ua+uCðð~/vÄu©@~ÜßrÄÄîß~.<¢h.rßßýöäbB¢<vB')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (9, 9, NULL, 2, 9, N'ã©_aB©¢/')
INSERT [#BugsDefectTracking] ([Id], [Number], [FailureId], [AssignedToId], [ResolvedById], [Comment]) VALUES (10, 10, NULL, 6, 10, N'.üÖ>Ü>+¢:ÄZß/>o ðª_Bu~|ßbhz@o+/üu¢Ã©ÐÖ ©/ü|C,Ð|ª@z>ö/|uã,ö.¢rä<bZz~ßß<~OZä£Ua/,*~Äýãå<_©.ãvß*Ã<~<>ãBoÖÜbÄ £|îüßü_bb<ä¢OßÖð,üußãª+bÜ@~zrãðöC_ßüÖ/AzUv£ýÐãzA u*ßßð>brärz+uår|î>¢üßOüöð_|ö+zðu|ýü<CCrbÜÜåuÐða,//ã ©b_b,äî+ðBß_ZBßUîÐ')
SET IDENTITY_INSERT [#BugsDefectTracking] OFF
/****** Object:  Table [#ConfigFailures]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#ConfigFailures](
	[ConfigId] [int] NOT NULL,
	[FailureId] [int] NOT NULL,
 CONSTRAINT [PK_ConfigFailures__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ConfigId] ASC,
	[FailureId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#ConfigFailures] ([ConfigId], [FailureId]) VALUES (11, 1)
INSERT [#ConfigFailures] ([ConfigId], [FailureId]) VALUES (12, 2)
/****** Object:  Table [#ThirteenNavigations]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#ThirteenNavigations](
	[Id] [int] NOT NULL,
	[BigInt_Id] [bigint] NULL,
	[Bit_Id] [bit] NOT NULL,
	[DateTime_Id] [datetime] NULL,
	[Decimal_Id] [decimal](16, 2) NULL,
	[Float_Id] [float] NULL,
	[Money_Id] [money] NULL,
	[Numeric_Id] [numeric](38, 18) NULL,
	[Real_Id] [real] NULL,
	[SmallDateTime_Id] [smalldatetime] NULL,
	[TinyInt_Id] [tinyint] NULL,
	[GUID_Id] [uniqueidentifier] NULL,
	[Varchar_Id] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SmallMoney_Id] [smallmoney] NULL,
	[DataColumn] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ThirteenNavigations__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#ThirteenNavigations] ([Id], [BigInt_Id], [Bit_Id], [DateTime_Id], [Decimal_Id], [Float_Id], [Money_Id], [Numeric_Id], [Real_Id], [SmallDateTime_Id], [TinyInt_Id], [GUID_Id], [Varchar_Id], [SmallMoney_Id], [DataColumn]) VALUES (1, 1, 0, CAST(0x00006FC600000000 AS DateTime), CAST(-0.01 AS Decimal(16, 2)), 3, 34.5600, CAST(0.000000000000000000 AS Numeric(38, 18)), 1, CAST(0xD6070000 AS SmallDateTime), 214, N'66666666-aaaa-7777-cccc-edededeedede', N'text2', 4.0000, N'yuck      ')
/****** Object:  Table [#TestScenarios]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#TestScenarios](
	[Id] [int] NOT NULL,
	[Name] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[ProjectId] [int] NULL,
 CONSTRAINT [PK__TestScenarios__30F848ED__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#TestScenarios] ([Id], [Name], [ProjectId]) VALUES (1, N'CßÜÃ~©£©>_Ö| uÜÜîBððv*ü~~CðÄBC: ~_BB@U~öÐö,C*@åA/Z~¢¢ßbÜãOB:Ðð|+B.üÜhÃU©@ýrzüãubUîvß@ãÃÜ<zv hCrß,aað|BîÖAðB£¢ÖßýüAuã©ÖävÄrzß.ð_.ß,ðöãZ>oý>ÖªÐboÄ>Z+Bî*h¢oÖu*ÜB*C,Ããuãäh©_', 1)
/****** Object:  Table [#Workers]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Workers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MiddleName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MentorId] [int] NULL,
	[OfficeId] [int] NULL,
 CONSTRAINT [PK_Workers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Workers] ON
INSERT [#Workers] ([Id], [FirstName], [LastName], [MiddleName], [MentorId], [OfficeId]) VALUES (1, N'Chris', N'Robinson', N'P', 5, 1)
INSERT [#Workers] ([Id], [FirstName], [LastName], [MiddleName], [MentorId], [OfficeId]) VALUES (5, N'Steve', N'O''Brian', N'l', NULL, 3)
INSERT [#Workers] ([Id], [FirstName], [LastName], [MiddleName], [MentorId], [OfficeId]) VALUES (6, N'Juan', N'Guitieriz', N'Carlos', 1, NULL)
INSERT [#Workers] ([Id], [FirstName], [LastName], [MiddleName], [MentorId], [OfficeId]) VALUES (8, N'Mark', N'Ashton', N'p', NULL, NULL)
SET IDENTITY_INSERT [#Workers] OFF
/****** Object:  Table [#Students]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MiddleName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Major] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CollegeId] [int] NOT NULL,
	[StudentType] [int] NOT NULL,
 CONSTRAINT [PK_Students__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Students] ON
INSERT [#Students] ([Id], [FirstName], [LastName], [MiddleName], [Major], [CollegeId], [StudentType]) VALUES (1, N'Ken', N'Rice', N'F', N'Civil Engineering', 1, 1)
INSERT [#Students] ([Id], [FirstName], [LastName], [MiddleName], [Major], [CollegeId], [StudentType]) VALUES (2, N'Emily', N'White', N'Anne', N'English', 2, 2)
SET IDENTITY_INSERT [#Students] OFF
/****** Object:  Table [#Recordings3]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Recordings3](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[DateOccurred] [datetime] NOT NULL,
	[OriginalSongId] [int] NULL,
	[OriginalArtistId] [int] NULL,
 CONSTRAINT [PK_Recordings3__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Recordings3] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 1, CAST(0x00009AEB00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings3] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 2, CAST(0x00009AEC00000000 AS DateTime), 1, 1)
INSERT [#Recordings3] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (2, 1, CAST(0x00009AEE00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings3] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (3, 2, CAST(0x00009A1700000000 AS DateTime), 2, 1)
/****** Object:  Table [#Recordings2]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Recordings2](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[DateOccurred] [datetime] NOT NULL,
	[OriginalSongId] [int] NULL,
	[OriginalArtistId] [int] NULL,
 CONSTRAINT [PK_Recordings2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Recordings2] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 1, CAST(0x00009AEB00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings2] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 2, CAST(0x00009AEC00000000 AS DateTime), 1, 1)
INSERT [#Recordings2] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (2, 1, CAST(0x00009AEE00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings2] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (3, 2, CAST(0x00009A1700000000 AS DateTime), 2, 1)
/****** Object:  Table [#Recordings]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Recordings](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[DateOccurred] [datetime] NOT NULL,
	[OriginalSongId] [int] NULL,
	[OriginalArtistId] [int] NULL,
 CONSTRAINT [PK_Recordings__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Recordings] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 1, CAST(0x00009AEB00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (1, 2, CAST(0x00009AEC00000000 AS DateTime), 1, 1)
INSERT [#Recordings] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (2, 1, CAST(0x00009AEE00000000 AS DateTime), NULL, NULL)
INSERT [#Recordings] ([SongId], [ArtistId], [DateOccurred], [OriginalSongId], [OriginalArtistId]) VALUES (3, 2, CAST(0x00009A1700000000 AS DateTime), 2, 1)
/****** Object:  Table [#ComputerDetails]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#ComputerDetails](
	[MachineName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OperatingSystem] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OperatingSystemVersion] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Status] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ComputerDetails__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[MachineName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#ComputerDetails] ([MachineName], [OperatingSystem], [OperatingSystemVersion], [Status]) VALUES (N'christrolaptop01', N'Windows XP', N'SP2', N'Ready')
INSERT [#ComputerDetails] ([MachineName], [OperatingSystem], [OperatingSystemVersion], [Status]) VALUES (N'christrotest02', N'Vista', N'Sp1', N'Ready')
INSERT [#ComputerDetails] ([MachineName], [OperatingSystem], [OperatingSystemVersion], [Status]) VALUES (N'christrotest03', N'Windows 2003 Server', N'Sp1', N'Unknown')
/****** Object:  Table [#Tasks]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Tasks](
	[Id] [int] NOT NULL,
	[ConfigId] [int] NULL,
	[Deleted] [bit] NULL,
	[RunId] [int] NOT NULL,
	[Started] [datetime] NULL,
	[Completed] [datetime] NULL,
	[StartedBy] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK__Tasks__1273C1CD__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (1, -1, 1, 9, CAST(0x0000A41201764804 AS DateTime), CAST(0x0009E983005338B0 AS DateTime), N'rªäh_*ã:Ö ~Ðü.ö©o~ Oa£ã./hßÜ©hzu©ã_î*UäöZãã>OãårU.:¢Z©Ü|Ö/Ä,@bÜ¢a*bCäbzðÄ,îoß UªUª@UÃÃöabýhð_<.rbu<ßÖÜ*rC_îîÜªÖ.OChð|@ö .bbåð++UüÃC|ä/ã<v hhÜAu:uÄÖ/*<£@Zo,ÃÐ<ZÐo©öb>O|Ðv£©U,ß|,ã~£r¢öî<A~ZÃAhÖÜb.ýCBhöboaoÃß.h>~aähãbBrÖ:AðîäÄCîårýß:ZÃ¢ªb¢ßu£¢/ÜCAZ¢öüzÜ*£BAhZzAßöuoÃhÄO>UA<.@ß.<CÖÃãvðuÃaB£~ª:Ö Ö/Ä,ZbüzChÄðß*ßãÃ|a@¢r@Aý:Ãbb~åÜvo©BBaObÜ+Ähuzä£vßîA/UÄ_ð©.ß,ðU@Uð£©ÃÐÖ©+üÐU<+ßÃªAÜvÃä*A:CO.ª*£ährîZ£rü_bu£:>:b_')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (2, -1546111383, 1, 10, CAST(0x001162C200DD80B0 AS DateTime), CAST(0x0008F8B601589D18 AS DateTime), N'bA åBZ:@A,bUr:rå_ýb¢_îCvÄo+@Äbý,ýÄO¢hª+ßB*ÃßzîÜ*OÃbu@ðßoîboðîößü.£v>Z|Öðªv_Orzüß,a aýÖåbîaÄB:/åh_Ä.©¢vb¢öhªv+¢öªB @å¢ßý£Ürªvã©<O+,£<åzOhöÄbäo¢ÃZbCýhîý@ÖÐÖå/©ßÜZ©/åýü:Äßh,öîÄh>o')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (3, NULL, 1, 8, CAST(0x0006BAD1004B8B74 AS DateTime), CAST(0x0000901A0000012C AS DateTime), N'ÖC.Ö<oðßrÐÖÃz~,rý@bªA~vÐð ~b¢zo>BbÄª+,ðßüððÜ.ßaª :*Ö väoã©ãh|åOå|U~uåßßªªU¢C©> C üÄoü,oãu_ÖÃ|ÜuBÐbrouÜÄ_¢ÖoB¢Ö>**ÄßªOaîoBÖa*Ahbßa_+ªzOå_UO_rUbu:A.*ãü¢îÐhAb:Ü+>>Ä*bb+îAOßv*a<Ãða__ ¢Ürã+££:vB/Üå©A>îýhbC+OaÄZîU/CbðuAäÄ>>Ã/Ð/Üb,<: /rAa,:rvªrhBÖ,AhA>|ãß a,zÄ Ã~Bªuß|ßUBb_ÖßªBZ+ª©*.¢Aîð¢@CO¢Ä©C@©@~îvr~ÜbýößAruÄaÄbýZübÄZrA©b: ä¢zö~Ä: <Ö¢~Aöh~ î<¢ý¢hUÄ:å¢boðÐ~©BOÄU¢a_,|u£+AÖoªÃÜ~ã*U>bUhßãUzãîý+äüåBZ:bªbî~ .C:::hhÖzãÃ©ý<OÄhU,Ö+£îrÐª>hÖÐO~<îÄÄ+ÜäÃÜUbavuBßZ+<ß>ãüÜrvÐ')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (4, 1386610270, 1, 7, NULL, CAST(0x00049F220140A780 AS DateTime), N'*,ÃýCªåð@@Ãã,v+b¢öö>BbabÖv<+OAaAßAC.ßäãO *r öo~ ZA|hßüuZ,£uäC@Ö>@©b*hövÜBÜOÖã©Cª©obh:äußßßAO¢ýbßªÄßvCuO*ÖÜAAß<:Ä_¢Oü/ªA©:u_zîBhCÐ+ohäã>/üÐ,©ßOa + £aßb*ý©oßå©ä/©£ðz/z äÃh')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (5, 9417404, 0, 4, CAST(0x002D247F018B81FF AS DateTime), NULL, N'ö~:h £ao:ÄAªÖBzÄAîä~Ab<öäbb£Üzz@Ã:ðäu>ª£.håh©_ýî.@ ö.ªOîC>ZZ~+,h,>îzO:<_zuoåo v bouvý+BbvAÐÐuzbbÖÐÐO¢©ÐüAÜ¢r:z')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (6, -1682641284, 0, 5, NULL, CAST(0xFFFF84480100BEF4 AS DateTime), N'AhÄÖ ýoBC/~BÄ+/vurÐîªüzö>vB¢ð¢~ÄÜÜ£~vZ:£b*OZrhuãýåîöZ>ubãÖUbî@O>ãåzöÐäaðÖBãvßaÖ*Av£bÄ¢/Z_~Ð>A,Ö_o©/ ßZBhäürr£üvãýîA¢åhÜu:A,hC>å**zBC|/_Ðo@OÖ.u¢~ î@h+C£AîuåÜîüa+Bää*åov|+Z<+üAßu.ßßuýÖýu£br |Öãr>ß£|+ZÄo¢öÜB¢Zbå.£+Bhß+*å¢Ãßööýª/Zäo+uãhB£~Aüåª*a îî,ÖbÃ/zÃÖ~îuÃüo£ .î>rßUý<u*+ýA ßªABuÃUOÐb_B_r/îbßuvðCãÄBhÖ|<böÄÐUB¢©äöüBUo/©ÄÄßbzv|ÖÖ,ÃÜzaBö©Ä£Cß+ZBßuråvOÃö,b+/ü@oß,öüä.ä*ääA._@ ã*.ÃUbbÃ>~ÄÐ+U¢£ýo_ãÄå,/©öå*aUÖo/buAavvbÖÄ:,hÃ<Curbvý©ªööýA')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (7, -1, 0, 7, CAST(0x002D247F018B81FF AS DateTime), CAST(0x000D4E8D00FBA1F8 AS DateTime), NULL)
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (8, NULL, 1, 8, CAST(0x00027B1C00EC2EE4 AS DateTime), NULL, N'Zý')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (9, 1644253482, 0, 2, NULL, CAST(0x0000901A0000012C AS DateTime), N'vbAUöäAbr~|+Ob¢| +@~ª@öÃAã/ªãb .|ãÖ@ª<öªbüu/ÐªZÃ<UäC£oäÐh rC./rhUoß*zCüä.î_£z@ü/|ÖCªrrÖ,ýA+ZvãOªÖBO|Üð++ZaÃÐªu/¢+Ö_©zªöA,/*ZÐÃ~~Ððåu ,©¢,Ð O@AÜÜ~Öî>Z@zohho*~b£ª.CaÃ uZüBîZ|vÖ@+£~ÃüÜbzu¢ÄbOUzbzBÄÜ,AB/+:ö.+uÐå,ªßÄý,©ß ßßb¢hBÄÜäý<|ýC _o ')
INSERT [#Tasks] ([Id], [ConfigId], [Deleted], [RunId], [Started], [Completed], [StartedBy]) VALUES (10, 2147483647, 1, 10, CAST(0x000C42A60106074C AS DateTime), CAST(0xFFFF2E4600000000 AS DateTime), N'ªbî.î/~UAßu*u¢@Uv .><oãboa£åßOß:_ZOßbb£_@b@*Bo~b.Ð¢<Zbü:uãvåßÄ/äýÖßö:ÐOrÃåü,£>üý.ª<b_ O: Ä|ußÖÖß££***£ª©|.ß.C|ßÐåv<OU+z©~+ªÐoö.ðCoÖýh¢£Üaß>h_Ü Öîß @Z||v*ÃÖýAä@¢aåZüßßªv¢ÄO:äOCßAäß ªå¢|~<£/ä£ÐhZåh£>+ðBubî~,C.O|ööOAOaCUÄ,*oo')
/****** Object:  Table [#People]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#People](
	[FirstName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MiddleName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PropertyValue] [decimal](18, 3) NULL,
	[PrimaryVehicleId] [int] NULL,
	[PersonType] [nchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_People__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[FirstName] ASC,
	[LastName] ASC,
	[MiddleName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#People] ([FirstName], [LastName], [MiddleName], [PropertyValue], [PrimaryVehicleId], [PersonType]) VALUES (N'Chris', N'Robinson', N'Patrick', CAST(1314.000 AS Decimal(18, 3)), 2, N'2')
INSERT [#People] ([FirstName], [LastName], [MiddleName], [PropertyValue], [PrimaryVehicleId], [PersonType]) VALUES (N'James', N'Robinson', N'A', CAST(323245.000 AS Decimal(18, 3)), NULL, N'2')
INSERT [#People] ([FirstName], [LastName], [MiddleName], [PropertyValue], [PrimaryVehicleId], [PersonType]) VALUES (N'M', N'Robinson', N'L', NULL, 3, N'1')
INSERT [#People] ([FirstName], [LastName], [MiddleName], [PropertyValue], [PrimaryVehicleId], [PersonType]) VALUES (N'Matt', N'Robinson', N'David', CAST(52255252.000 AS Decimal(18, 3)), NULL, N'2')
INSERT [#People] ([FirstName], [LastName], [MiddleName], [PropertyValue], [PrimaryVehicleId], [PersonType]) VALUES (N'Steve', N'OBrian', N'm', NULL, 5, N'1')
/****** Object:  Table [#OwnerOwnerDetails]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#OwnerOwnerDetails](
	[OwnerId] [int] NOT NULL,
	[DetailId] [int] NOT NULL,
 CONSTRAINT [PK__OwnerOwnerDetail__07020F21__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#OwnerOwnerDetails] ([OwnerId], [DetailId]) VALUES (1, 1)
INSERT [#OwnerOwnerDetails] ([OwnerId], [DetailId]) VALUES (2, 2)
INSERT [#OwnerOwnerDetails] ([OwnerId], [DetailId]) VALUES (3, 3)
INSERT [#OwnerOwnerDetails] ([OwnerId], [DetailId]) VALUES (4, 4)
/****** Object:  Table [#MachineConfigs]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#MachineConfigs](
	[Id] [int] NOT NULL,
	[Host] [varchar](512) COLLATE Latin1_General_CI_AS NULL,
	[IP Address] [uniqueidentifier] NULL,
 CONSTRAINT [PK__MachineConfigs__00551192__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (1, N'öÜüBåãZã~a©ðåv<ßßÖzýzbbÃ ¢bß¢ä|ª> ¢.åö|>Ã@ßãýb.å £|,å AåÄvÖvÖ>uÐ/b._£bðãOßZßUuÄvur:üZOÐoZÐÃªOzÖrzB,î£äuUÐÄrbAU¢<_>A|AU:U UUî+z©äbB>oßß¢*ürU|ý£hbCubÃvrob¢o|,¢ å.b AðÖ<©ÐåÃ~ßaåv©Cr', NULL)
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (2, N'ü~äð/@©UuÐ£ ß>£¢rz£|zß|ª*AÐååv.@ÐhbÐOazöý>£üö>O+ªbðåo~üoAU~,ÜÜ*üzhª©/î:raÃ|ßoª©ãä*vã*_*hOðÄð.îouhÃ©ßî>öÄ~Äå:Ahý.|Ð_/>>ªÖÜ:>@O@ÜB_ ©<ðäö*.|', NULL)
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (3, N'îBAðz+©+ÐÐ¢bîA/a/@böÜîÃ<>>ÄßraðvüBä*:~åoBÐßªA.aß> üAO©åý.|.ß£åOÄß+Bßob_ã£îäbBb/B<åzäbîbZuåöîOb>u|Cß©zr£.ßC,vßßî*C  Ü~z£åzß~Zh£©|.¢>üUÜÜvuåbö>ß<.üªÜäÖÐ£U:ö+@¢+Zh/ãärzðB _b|ÄUÄ+> oz@äßßÃýv@z.U£ä~ð>©öZBb>/zÄoöýÖ|är:bÖZªAOrb.ÜðZ,ÄC rÄ|bbvA.@b@bvÖÖ_brßß¢å£Öbb¢Ð@a :ß£_äzh,ÜÐäBö©oÜ£A+UrîzÃ bßüa£zå@Öbu¢uîUðßabÜßÃBðZüÃoÜC|@öuÜ@o,h/ü~rªðh/hhz.åuð', N'99999999-9999-9999-9999-999999999999')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (4, N'bÄ©ªÐÖ¢>Ã©aCÄÖ,å£ßu<>ýh_|A:Ä©zaÃ,ãC©:ãUã|Cu:.a.|aa|hzîrb~hAÄZðß,© Äå£Ä.ß_,î+Ö©C|¢åÃ£Ðå+>£ABzß__£zãßBZ@ðz~Ö~ðýß îA<B.uo£ÜöuUªªCaaãvbîäavBö,býZ/>ßÄrZz:h.hOahbv Ã<Ö|bÄÐ.Aauý:ãC©<>Ä/î.ßÃ|ÄãöÐãåOß¢Ã|uÐ+Übzovb:Ã~a@_¢*ã.v©bý+åßÃOhAoßh_/Öhð*~v_ªCAvðuzUãýr/îOÐýîZ¢ühbzä:<bÜ>/>ßaC/å ªä:äårOªÄ¢vAhabüZ.|ßoß:h_Ö.<+.ßÖã©z.oBUãCvÖãßÐ*./äÐÄäBOß.z hßÄuZã>r<Ä£ßOoÄÃîbª ', N'1b988b4e-26ad-4d63-99cf-2b48e45114f1')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (5, N'A@ÃBåÖ>>aãÜCb.A:az_åÃßA__h,vÐßý@Ðß©ÄªÖª:ýu¢ý,A©våªªAv@zU>h_Ö*uÖräv+UÖ äUÃbå<obh>:C,hröUu>/r:räBÖ:/hC£.@_Ö**r¢ßb:ýAZuã¢ýîAoÖ C,bãýhaÄîaäA~ã,ãªA~©*Ö:o|äðß/uB<©r.*z@r>ß*_üOU¢ýUozý~Bü+öåvCrOZÃð++,.b_Bãbäýu<ý uö~ZðBî|Ub,.UBZ¢bãbßåU|rªro>Uüh:îöåvªýÜã:ßh*£CãÐßuð+/o:ý+,©Äð. vðö@zuAabbu*bCr|>+/ÄabÖAÐ<îårzbîb<å/Ã~@>Öuãzuß,äOãO*>Ãzroðßß hßÜa z~uÖäðãðößu:ßðBAÄ+hÄ,*o©vÜÜÃOª.bzî>ß£.üv>~Ö@ýÄCåÐz//¢Zðö._>BhhÐ*,ohî.Z,Z.åußª£Ã_£åüåoý/aü|,BðåC+ã.hß~r</ðoAÖbC Öäö/UoäO.ÄCöäîZ+@Öäür@åü/._üaöÃöäu¢Uý/årýuÐB:v¢@¢Ü>ýrhhUýüýßªåo:Aob¢', N'99999999-9999-9999-9999-999999999999')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (6, N'Bä£ýüÜÜ@OßA*u:ÃÖÄB+va/b~BbZbZrýU|,,h_ho<u/ßA~_öý*.*_åß+£ßbîî>ÃUýßÜÜßb|r|ðîb|¢Aa_CzÐäã/*ÜAÐ <UUü@rãrßÜCîßrÜ¢üÖßAÜb*Ürü+h,.ýß UÄ Ãö@+ÜßuÖa,ö£bß+AuÄ_/å:Ü|ãÃß+ßz~_>:ðAu/UüßªÜßaÄb~bü~¢ÃZ/uoü_býäüÄß,oªå*ßÜ', N'99999999-9999-9999-9999-999999999999')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (7, N'ZÃü_CªU ãÄüäuAÃävîoüü¢ba¢_<ÃÃoßoC©h_r..<Ö@hýã_ö|O<_åZ¢££ªöð|C_¢©ðßÐ_åh©ã<Ãr/>BOBªð@u¢ðä>a<ðU>ã*rî:bAß,©AßÃ,ª.ro@ÜßürA.býöU_,u.*¢*ý_AAoZuU<,ö@vbß .<©åhO<.bb_ävß,*ÖU.©C>¢@Öü|aC<ÃOÐbßö©Öußb¢übÖ*ü£ý~Öa£ouîãohOª<z~,+hra:ÃaAªª<__Ü,ýîa@Ä©@|ãîB>ªAu~z+Ã+uÖ¢_îÖüb_Ð+z+ÖrÃ:ßåÄä|+C+O.a+AOoä.ouZ<ªöÄ:AB_ßhäã@C@<ü@©,@OÐ*ü©@|r/ü~ü.', N'47a725b9-0c69-491f-a589-397047961d56')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (8, N'ZOzî.>hzOÃ>/öÖ>üãÖ¢öý..ZB|Ö+a_+@<vÜA££|ß _BÃZ,îÖzß|ßB¢ßäÜaßäÐÖz£züü©ü:zOrßª: ÄýÄ£ýbrü|/ã<ðU£ãÄb~u:+ /ußã<+ªvA*_babrÐÃB©Ö_Crbu©*<ÖAuä*©Z/rUÐ~öoßh+r¢Ü> åhÜªßzaÖ¢ßAC@ýC©Ä*ßÖ@:+bß,zb©aA|ý.hBbÐåü.:¢î_ß£ßaÃ>ZüÐß/ÜÄß.aöbªß<hBäAªü:ß*bß:@ýÐ<bü@ÐªUªÃz+ö<ü<,ACb©_üüB_ßh,ÐªzOß©_äBbbäî<oÄuaýäåÃß : O@Båýî:äåbaOoß/ªZöbuÜbßvå@ßv>bßüÐ<<aÜubh.bªbÄAäO/Z,ãZu|CÄãý<bO*ßvA:Ã .ßbåîßÄ:Ð_ã©£ßu£üÃ>î*zðuªbuÄÜß©~OãA: ü+.ã£ýZoß/*h', N'99999999-9999-9999-9999-999999999999')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (9, NULL, N'99999999-9999-9999-9999-999999999999')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (10, N'_v~|A_oäî£Ãåå>ßÜZåªÐß>_ *OvåCC*|üChhðªÃßßß¢î@|bBh£ÃZ ZªAÄUuäZZÐßzhðÖ+ÃãÃrßðüå¢ßßBOÐ<.buÃ@B.©,ß@@bã~+¢zÄ/ßaB@+¢oB/.ÖB|v+vîuUZZoz+Z,åbb/oß<Z©.üÄ îð Uüb.üß: <,b/o£ÄÄuüÐCAývü¢v~üZOî +üî zä.b~ÄüÖÐACAÃU>*o', N'00000000-0000-0000-0000-000000000000')
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (11, N'ä£AB¢Cv:ß*bbÖ*h£,üzðßÄüÖü¢îA£ýð/Avãb©aZ+Üß|>ßzÐÄbÄh¢+¢:oÃvCýuußß.¢a<>Bý+Ãðoýo<ü£årüöäzb,ßÃ*,îZo_aÜbozUrüÄÖð/<:arz:BaväðB|ª©ßÖ~:,Üªü+,ß*¢ýý', NULL)
INSERT [#MachineConfigs] ([Id], [Host], [IP Address]) VALUES (12, N'@ rä.oðÃräzrößÄzboßöäb,ð/+', N'00000000-0000-0000-0000-000000000000')
/****** Object:  Table [#RunOwners]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#RunOwners](
	[RunId] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
 CONSTRAINT [PK__RunOwners__0EA330E9__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[RunId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (1, 8)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (2, 5)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (3, 3)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (4, 4)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (5, 4)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (6, 7)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (7, 3)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (8, 2)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (9, 4)
INSERT [#RunOwners] ([RunId], [OwnerId]) VALUES (10, 7)
/****** Object:  Table [#Run3s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Run3s](
	[RunId3] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Run3s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[RunId3] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Run3s] ([RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, N'Run1', N'christro')
INSERT [#Run3s] ([RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, N'Run3', N'steveob')
INSERT [#Run3s] ([RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, N'Run2', N'davebarn')
/****** Object:  Table [#Run2s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Run2s](
	[RunId2] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Run2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[RunId2] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Run2s] ([RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, N'Run1', N'christro')
INSERT [#Run2s] ([RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, N'Run3', N'steveob')
INSERT [#Run2s] ([RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, N'Run2', N'davebarn')
/****** Object:  Table [#Run1s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Run1s](
	[RunId1] [int] NOT NULL,
	[BuildID] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Run1s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[RunId1] ASC,
	[BuildID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Run1s] ([RunId1], [BuildID], [Name], [LabOwnerAlias]) VALUES (1, 1, N'Run1', N'christro')
INSERT [#Run1s] ([RunId1], [BuildID], [Name], [LabOwnerAlias]) VALUES (1, 2, N'Run3', N'steveob')
INSERT [#Run1s] ([RunId1], [BuildID], [Name], [LabOwnerAlias]) VALUES (2, 1, N'Run2', N'davebarn')
/****** Object:  Table [#OldVehiclesLinkTable]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#OldVehiclesLinkTable](
	[FirstName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MiddleName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[VehicleId] [int] NOT NULL,
 CONSTRAINT [PK_OldVehiclesLinkTable_1__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[VehicleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#OldVehiclesLinkTable] ([FirstName], [LastName], [MiddleName], [VehicleId]) VALUES (N'Chris', N'Robinson', N'Patrick', 2)
INSERT [#OldVehiclesLinkTable] ([FirstName], [LastName], [MiddleName], [VehicleId]) VALUES (N'Chris', N'Robinson', N'Patrick', 3)
INSERT [#OldVehiclesLinkTable] ([FirstName], [LastName], [MiddleName], [VehicleId]) VALUES (N'Steve', N'OBrian', N'm', 4)
/****** Object:  Table [#TaskResults]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#TaskResults](
	[TaskId] [int] NOT NULL,
	[Passed] [bigint] NULL,
	[Failed] [bigint] NULL,
 CONSTRAINT [PK__TaskResults__15502E78__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (1, 1337821826, 1080170422)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (2, 9223372036854775807, -907360679)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (3, 1648262218, 438935329)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (4, -9223372036854775808, -2806820883)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (5, -1699990667, -9223372036854775808)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (6, -1, 852247751)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (7, 1, -1)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (8, 1475038239, -9223372036854775808)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (9, -1169619645, 85150152)
INSERT [#TaskResults] ([TaskId], [Passed], [Failed]) VALUES (10, 0, -825265197)
/****** Object:  Table [#TaskInvestigates]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#TaskInvestigates](
	[TaskId] [int] NOT NULL,
	[Investigates] [bigint] NULL,
	[Improvements] [bigint] NULL,
 CONSTRAINT [PK__TaskInvestigates__182C9B23__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (1, 3097925411, 1241558469)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (2, -1321755389, 9223372036854775807)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (3, NULL, 9223372036854775807)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (4, 188625021, -1)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (5, NULL, 1310128077)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (6, 118921550, -1608923773)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (7, 1, 221864504)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (8, 3187034607, 297739599)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (9, -817922720, -1030528598)
INSERT [#TaskInvestigates] ([TaskId], [Investigates], [Improvements]) VALUES (10, -648528035, -3859554120)
/****** Object:  Table [#RecordingAlbumsLinkTable3]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#RecordingAlbumsLinkTable3](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[AlbumId] [int] NOT NULL,
 CONSTRAINT [PK_RecordingsAlbumsLinkTable3__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC,
	[AlbumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#RecordingAlbumsLinkTable3] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 1, 1)
INSERT [#RecordingAlbumsLinkTable3] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 2, 1)
INSERT [#RecordingAlbumsLinkTable3] ([SongId], [ArtistId], [AlbumId]) VALUES (2, 1, 2)
/****** Object:  Table [#RecordingAlbumsLinkTable2]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#RecordingAlbumsLinkTable2](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[AlbumId] [int] NOT NULL,
 CONSTRAINT [PK_RecordingAlbumsLinkTable2__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC,
	[AlbumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#RecordingAlbumsLinkTable2] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 1, 1)
INSERT [#RecordingAlbumsLinkTable2] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 2, 1)
INSERT [#RecordingAlbumsLinkTable2] ([SongId], [ArtistId], [AlbumId]) VALUES (2, 1, 2)
/****** Object:  Table [#RecordingAlbumsLinkTable]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#RecordingAlbumsLinkTable](
	[SongId] [int] NOT NULL,
	[ArtistId] [int] NOT NULL,
	[AlbumId] [int] NOT NULL,
 CONSTRAINT [PK_RecordingAlbumsLinkTable__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SongId] ASC,
	[ArtistId] ASC,
	[AlbumId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#RecordingAlbumsLinkTable] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 1, 1)
INSERT [#RecordingAlbumsLinkTable] ([SongId], [ArtistId], [AlbumId]) VALUES (1, 2, 1)
INSERT [#RecordingAlbumsLinkTable] ([SongId], [ArtistId], [AlbumId]) VALUES (2, 1, 2)
/****** Object:  Table [#Test9s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test9s](
	[TestId9] [int] NOT NULL,
	[RunId3] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test9s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId9] ASC,
	[RunId3] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test9s] ([TestId9], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test9s] ([TestId9], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test9s] ([TestId9], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test9s] ([TestId9], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test8s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test8s](
	[TestId8] [int] NOT NULL,
	[RunId3] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test8s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId8] ASC,
	[RunId3] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test8s] ([TestId8], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test8s] ([TestId8], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test8s] ([TestId8], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test8s] ([TestId8], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test7s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test7s](
	[TestId7] [int] NOT NULL,
	[RunId3] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test7s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId7] ASC,
	[RunId3] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test7s] ([TestId7], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test7s] ([TestId7], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test7s] ([TestId7], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test7s] ([TestId7], [RunId3], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test6s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test6s](
	[TestId6] [int] NOT NULL,
	[RunId2] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test6s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId6] ASC,
	[RunId2] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test6s] ([TestId6], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test6s] ([TestId6], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test6s] ([TestId6], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test6s] ([TestId6], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test5s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test5s](
	[TestId5] [int] NOT NULL,
	[RunId2] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test5s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId5] ASC,
	[RunId2] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test5s] ([TestId5], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test5s] ([TestId5], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test5s] ([TestId5], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test5s] ([TestId5], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test4s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test4s](
	[TestId4] [int] NOT NULL,
	[RunId2] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test4s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId4] ASC,
	[RunId2] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test4s] ([TestId4], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test4s] ([TestId4], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test4s] ([TestId4], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test4s] ([TestId4], [RunId2], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test3s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test3s](
	[TestId3] [int] NOT NULL,
	[RunId1] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test3s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId3] ASC,
	[RunId1] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test3s] ([TestId3], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test3s] ([TestId3], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test3s] ([TestId3], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test3s] ([TestId3], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test2s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test2s](
	[TestId2] [int] NOT NULL,
	[RunId1] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test2s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId2] ASC,
	[RunId1] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test2s] ([TestId2], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test2s] ([TestId2], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test2s] ([TestId2], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test2s] ([TestId2], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
/****** Object:  Table [#Test1s]    Script Date: 11/17/2008 11:22:02 ******/
CREATE TABLE [#Test1s](
	[TestId1] [int] NOT NULL,
	[RunId1] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LabOwnerAlias] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Test1s__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[TestId1] ASC,
	[RunId1] ASC,
	[BuildId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Test1s] ([TestId1], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 1, 2, N'Test1', N'christro')
INSERT [#Test1s] ([TestId1], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (1, 2, 1, N'Test3', N'steveob')
INSERT [#Test1s] ([TestId1], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 1, 2, N'Test2', N'christro')
INSERT [#Test1s] ([TestId1], [RunId1], [BuildId], [Name], [LabOwnerAlias]) VALUES (2, 2, 1, N'Test4', N'davebarn')
