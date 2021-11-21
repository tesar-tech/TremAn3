%% simple data
a = [0.101264,-0.046128,0.101264,-0.046128,0.101264,-0.101697,0.101118,0.303405,0.453618,0.76023,0.401859,-0.571156,-0.979348,0.118543,1.179861,1.140436,0.964927,0.736208,0.33818,0.037235,-0.279469,-0.666263,-1.321354,-1.689606,1.469276,-1.83178,1.599573,0.701195,0.029702,2.698685,-0.227614,0.192962,-0.464314,-1.292149,0.420356,-0.930825,-0.388576,2.061032,1.382679,2.088774,2.014803,-0.246264,-0.282266,0.155661,0.57581,-1.642372,-2.250372,-1.639838,-2.420215,0.684765,0.484173,0.72842,0.670495,1.247411,-0.285939,-1.112207,1.069814,-2.401283,0.8525,0.238972,1.923099,2.142347,2.772915,-0.961987,0.940643,0.216243,-1.155571,0.707564,-1.704363,-2.741205,-2.040627,-0.702846,-1.037942,2.876061,0.172974,-0.282197,0.349473,0.172019,0.792469,-1.132277,-0.192295,-0.377563,1.79874,1.807919,2.637783,0.250194,1.177264,0.546754,0.040738,-0.012887,-1.836195,0.384701,-3.339677,-2.361179,-1.363699,-0.12295,1.637589,0.319005,0.673747,1.066807,-0.334019,-1.108073,0.966536,-2.372043,0.140823,1.310695,2.092464,3.135178,0.239482,1.568811,0.498452,-0.754028,0.513737,-0.613899,-1.776461,-2.689232,-2.206949,-2.825139,0.782429,2.774491,-0.506842,0.815433,0.737827,-0.970548,0.453872,-1.692485,0.361202,-1.201583,1.953353,1.501673,2.217532,1.102104,-0.561707,-0.257441,0.255171,-1.41055,-2.206209,-0.934331,-1.691263,1.533396,-1.360695,4.137105,-0.949755,2.222316,-0.479235,0.347976,-0.204037,-0.514362,-2.664919,0.789622,0.656021,2.468732,2.942605,0.887553,1.435171,-0.672555,0.094631,0.22055,-2.953088,0.078282,-3.23897,-1.723228,0.103497,-1.336996,3.777191,1.13831,-0.929283,-0.426814,0.694214,-1.432348,0.684217,-1.622008,-0.839732,2.730974,4.032711,2.119581,-0.734432,-0.589741,0.165286,-0.099108,1.780798,-4.443636,-1.619146,-2.096325,-0.713006,-0.635578,3.497324,0.133617,0.674991,-0.762151,0.332501,0.05836,1.206391,-1.503944,-2.685616,1.146625,0.402181,2.483232,2.141463,3.627768,-1.611522,0.458747,0.135906,-0.753007,1.047205,-1.672734,-1.632148,-3.543072,-1.767492,1.363962,-1.080069,3.791387,-0.260535,1.524884,-0.687335,0.477051,0.466167,-1.511867,0.750954,-2.187629,-0.689923,1.956688,2.00691,2.795666,1.808991,0.38933,-1.220206,0.523919,-0.133843,-0.616607,1.301153,-2.117575,-1.489843,-2.862567,-1.421879,-0.226355,-0.019844,1.355693,1.528044,1.073483,0.434075,-0.972146,-0.015658,0.271431,0.813846,-0.625405,0.36691,-3.32366,1.648804,-3.012971,2.651024,3.424327,-0.104336,2.477836,0.00706,-1.825663,-3.081234,-2.346887,-1.640275,-0.381413,0.141239,0.865633,1.101479,0.621771,0.85839,0.471711,-0.000804,-0.649837,-0.039852,6.093333,-5.401255,-14.197873];
b = [-0.049748,-0.09789,-0.167551,0.166693,0.409357,0.382399,0.764531,1.034092,0.801341,0.78378,0.814601,0.443658,0.439475,0.435551,0.586829,0.569659,0.019037,-0.57476,-0.677995,-0.77569,-1.345612,-2.092216,-2.098034,-1.367582,0.075584,1.998385,-0.165323,-0.405388,0.485171,0.705281,-1.665356,1.081766,1.993,2.577929,1.283744,2.251388,1.075,1.187513,0.175013,-0.414455,0.324768,0.311845,-1.64002,-0.724964,-2.531288,-1.829561,-2.518977,-0.983029,0.147659,0.879408,-0.068932,0.135327,-0.652712,0.182426,1.2243,2.341852,1.679471,2.289782,2.121378,1.57493,0.041193,-0.412997,0.132208,0.230997,0.630246,-2.638044,-1.252235,-3.034227,-1.539896,-1.646467,-0.767222,-0.237509,0.741925,0.121728,-0.300489,-0.10692,1.135907,1.769216,1.97722,1.005152,2.516832,0.495594,0.2312,-0.426917,0.845849,0.511474,-0.420765,0.123281,0.199949,-1.509367,-2.731013,-1.209802,-1.611487,-1.648269,0.240162,0.36774,-0.268269,0.418767,-0.401126,0.004044,1.700211,2.11564,0.920688,3.656072,1.418629,1.249035,-0.460961,0.549732,0.397718,-0.138917,-0.761843,0.415397,-2.62718,-3.33242,-1.855297,-2.125932,-1.141721,0.370086,0.586713,0.092446,-0.259599,-0.697176,0.898214,1.494758,1.432983,1.442722,3.104185,0.934906,2.032878,0.51529,0.14053,-0.318031,-2.387369,-1.506636,-3.084022,-1.731367,-1.17125,-0.610011,0.098343,1.206359,-0.785462,-0.232469,0.08713,0.914813,-1.124003,0.808165,1.936849,1.494293,4.115929,2.524358,-0.415624,0.906183,0.649057,-0.034073,-0.864319,-0.54059,-0.682187,-2.825393,-4.034102,-1.893248,-1.203299,1.388783,-0.058911,0.242787,0.430162,-1.066961,1.291873,-1.810124,1.324783,1.976255,5.058624,3.221863,0.328344,1.657027,0.207919,0.179038,0.358989,-1.447,-0.729616,-3.715481,-4.140353,-2.4825,-1.454827,0.914374,1.555886,-0.281741,1.52677,0.413459,-1.007562,-1.100386,0.082474,-0.272361,1.956555,2.172125,4.47546,3.342306,0.349031,1.640758,-0.288563,-0.103937,0.412955,-0.809276,-2.138563,-1.214666,-3.775978,-3.915838,-1.371966,-0.707136,0.956259,0.340807,-0.651652,0.291897,-0.339028,0.078047,0.654325,-1.165763,0.916942,1.715848,2.197145,3.679653,3.166814,0.096376,1.720387,0.44852,-0.329299,0.139968,0.352447,-0.979407,-2.147635,-0.626144,-3.601111,-3.857543,-1.2661,-0.85287,-0.287116,1.207148,-0.175813,-0.158428,1.197595,0.636728,-0.140381,-1.293561,0.289818,0.237843,0.146534,1.201726,1.221556,2.660045,2.038041,0.665928,-1.255297,-0.194761,-0.094786,0.012566,0.696038,-1.194337,-2.395401,-0.615664,-0.05333,0.24718,0.187371,-0.014014,0.089428,1.010731,1.646981,1.263208,-5.719237,-11.722363,-4.466629,1.156828,1.100303,1.463467];
%% data from x and y com movement
a_y  = [0.424713,0.265215,0.152077,-0.3978,-0.437105,-0.403738,-0.7618,-0.761208,-0.502849,-0.222719,-0.236149,-0.1043,-0.321688,-0.193232,-0.045591,0.027922,0.240359,0.30234,0.48431,0.622576,0.762886,2.16901,2.512941,2.363825,1.735414,0.881779,0.560585,0.167053,-0.119819,-0.837948,-1.066766,-1.582287,-2.993196,-2.382768,-0.723932,-0.723113,-0.815007,-0.56996,-0.345415,-0.273941,-0.127801,0.159479,0.380165,0.783205,1.075381,2.248134,3.557625,3.006968,1.225131,0.985408,0.286535,-0.017037,-0.775659,-1.07476,-2.481996,-3.145739,-2.248076,-1.684087,-1.25708,-0.766767,-0.366798,-0.155767,0.015346,0.136567,0.259823,0.38374,0.894036,1.536562,3.369001,3.071986,1.860984,0.534726,0.648507,0.316431,-0.323135,-0.335572,-0.601484,-2.496575,-2.620989,-1.320958,-1.246913,-0.961987,-0.694937,-0.453197,-0.260647,-0.462844,-0.271899,-0.127366,0.435546,0.926068,1.543452,3.591342,3.058764,2.388149,0.867888,1.213714,0.35861,-0.139971,-0.859806,-0.554414,-3.166348,-3.320581,-2.717569,-1.762829,-1.026051,-0.49977,-0.099989,0.059039,0.150575,-0.092533,0.058866,0.458434,0.979919,1.778997,4.003164,5.093009,3.089214,0.772591,0.970524,0.26948,-0.792255,-0.567716,-1.547597,-2.997752,-3.229098,-2.081286,-1.671717,-1.015164,-0.5218,-0.403087,-0.143688,0.304385,0.528217,0.926102,1.604603,3.940047,3.772193,3.295397,1.299303,1.762778,0.210277,0.078765,0.059051,-0.150963,-1.489319,-2.081584,-4.5323,-4.494389,-2.383706,-1.16345,-0.535359,-0.130541,-0.056118,0.067992,0.208736,0.38209,0.753813,1.313193,3.07643,4.933938,4.743398,1.573503,1.238335,-0.08394,-0.354496,0.023196,-1.083364,-1.60326,-4.944085,-5.802891,-2.934662,-1.384763,-0.687909,-0.51563,-0.179629,-0.110065,-0.023798,0.194377,0.519502,1.065281,2.638719,5.324793,5.202359,2.764556,1.952526,0.07647,0.563048,0.273888,-0.118606,0.119183,-1.430513,-2.199934,-5.355833,-5.117921,-2.508901,-1.213889,-0.554274,-0.439134,0.060538,0.0601,0.164046,0.1896,0.531914,1.266289,1.974791,4.499818,4.84513,3.924499,1.303963,1.423762,0.087819,0.225973,-0.006343,-0.004962,-1.027347,-1.069262,-3.205601,-4.191516,-3.931434,-1.712975,-1.291666,-0.827403,-0.212238,-0.120655,0.031285,-0.127926,-0.03058,0.266888,0.483004,0.97926,1.790255,3.471172,4.44533,3.70748,2.144567,1.574772,0.927055,0.030304,0.40657,0.644983,-0.504076,-0.148768,-0.056867,-1.815395,-1.32613,-4.795632,-4.436352,-4.654759,-2.046029,-1.014446,-0.842088,0.019458,1.932568,1.723548,1.826668,1.644871,1.910056,0.818087,1.020321,1.201682,0.346569,-0.159937,-0.300059,-1.345584,-2.56582,-1.135246,-5.035152,-1.619946,3.066971,-0.457545,-0.502421,0.393353,-0.023714,-0.686125,-0.03021,0.064849,-0.109426,-0.217772,-0.566722,-0.730471,-0.957453,-1.097431,-0.686549,-0.777382,-0.312633];
a_x = [0.416646,-0.038365,0.037146,-0.609517,-0.377726,-0.311601,-0.373949,-0.345319,-0.010358,0.456603,0.660029,0.835564,0.840453,0.704586,0.692448,0.381667,-0.145872,-0.593848,-0.918662,-1.26847,-1.588409,0.396707,1.333255,3.179714,0.933441,2.192812,1.103337,-0.419492,-0.123807,-1.020785,-2.825414,-0.781456,-3.358617,-0.101393,2.230557,2.340862,2.514339,1.951377,1.099868,0.284402,-0.662287,-1.578806,-2.585481,-3.106769,-2.817484,-0.940508,1.551101,3.156566,0.343904,1.86653,0.382799,-0.60513,-2.033345,-0.730639,-3.046692,-1.518588,0.581643,2.381746,2.959071,2.855714,2.049547,0.741182,-0.225681,-1.220812,-2.145538,-2.989409,-3.632601,-2.443729,0.809145,1.914843,2.059006,-0.097469,1.436391,0.457215,-1.299839,-0.846572,-0.179568,-2.242621,-0.365026,1.727804,2.548052,2.647884,2.12853,1.105251,0.464242,-0.139135,-1.058145,-2.022907,-3.158269,-3.596849,-2.817692,0.397724,1.270301,2.358321,-0.006814,2.209809,0.301262,-0.469835,-2.095958,-0.410188,-3.116581,-1.319558,0.992629,3.118311,3.460331,2.936415,1.469566,0.79007,0.134141,-0.884497,-1.619389,-3.054134,-3.912168,-3.979147,-0.398324,2.498366,3.701165,-0.129587,1.833421,0.240328,-1.602799,-1.178384,-1.048362,-2.820553,-0.958858,1.51661,2.484749,2.191651,1.519719,0.557719,-0.618678,-1.599053,-2.374235,-2.777746,-2.462187,0.74877,1.564718,3.51976,0.456758,2.38112,0.704316,-0.547676,0.300886,-0.210512,-2.964586,-1.479463,-3.285664,-0.115474,4.040995,3.848286,2.415864,1.057016,0.503842,-0.686685,-1.211438,-2.015875,-3.293508,-3.776457,-1.797425,1.541607,4.841726,0.628801,3.337408,-1.192145,-0.305813,0.471779,-1.375791,-2.044437,-5.372738,-1.637426,3.712902,5.298367,3.745361,2.447803,1.224961,-0.232753,-1.828679,-2.456908,-2.867124,-4.416249,-3.840569,1.567443,4.335883,2.414615,3.120733,-0.625062,0.663168,0.50715,-1.127592,0.666993,-3.221282,-1.514494,-4.525189,-0.66167,4.655331,4.83531,2.895047,2.221953,1.305561,-0.126443,-1.530242,-2.385564,-2.107173,-3.541803,-4.214276,-0.057906,2.107504,4.179033,0.341743,2.564413,0.009438,-0.730751,0.563069,0.511125,-2.475167,-0.838348,-3.92371,-1.965832,0.112926,3.631795,4.203868,3.181872,1.550706,1.39201,0.709865,-0.395086,-1.770334,-1.921029,-2.281155,-3.366729,-3.753508,-0.98219,1.577878,2.652308,2.233854,0.89366,2.018922,-0.584004,-0.490019,0.430977,-0.616592,-0.132257,0.429646,-3.062302,0.430431,-4.151702,-0.235504,0.691848,2.128219,0.221592,-0.485909,-1.017055,0.390245,-0.122401,-0.148848,-0.129732,0.592623,0.273996,0.302379,0.413428,0.326823,-0.102714,-1.175515,-0.92572,0.520655,-0.905312,0.826548,5.960869,0.447651,3.397748,1.014237,1.364691,0.157,-1.044556,0.181346,-0.518963,-1.706826,-1.190439,-1.444793,-1.90586,-2.851112,-2.522537,-0.171161,0.555813,0.583888];
aa = sqrt(a_x.^2 + a_y.^2);%vectors

b_x  =[-0.22596,-0.123774,-0.15194,0.408703,0.352812,0.269766,0.387227,0.795854,1.245369,1.436885,1.069047,0.223925,-0.590591,-0.48669,0.242269,0.185519,-0.011964,-0.308304,-0.477667,0.003532,-0.520031,-3.762012,-1.637111,-3.931227,-0.764007,0.596735,1.312398,0.21548,0.15917,-1.080546,-0.346171,1.615471,3.894517,2.499598,0.051352,1.25589,-2.728712,-0.328113,-0.087773,-0.01394,0.403242,0.166722,0.886043,2.530905,-2.06704,-0.757899,-3.388094,-3.364418,0.561256,0.853539,0.27798,-0.402452,-0.445298,-0.326712,3.310982,2.043512,2.367947,1.393804,-1.611139,-1.308312,0.014191,-0.352081,0.328999,0.604821,-0.012242,1.639927,1.530483,-3.395488,-1.667229,-2.176845,-2.41935,0.388996,0.519359,0.228204,-0.207827,0.104172,1.522447,1.848653,1.995962,-0.384931,0.476654,-1.965409,0.677366,-0.427641,-0.378877,0.328576,0.938136,0.335292,0.202012,2.927042,-2.901869,-0.285793,-2.689761,-2.145669,-0.154259,1.361907,0.389427,-0.36988,-1.341235,0.394904,2.793721,2.35439,1.045316,1.637512,-2.973836,0.252269,-1.226056,-1.345178,-0.544074,0.841395,1.668822,0.300248,1.89121,-0.316466,-1.713565,-3.838763,-2.696143,0.228111,1.339958,0.26179,-1.348453,-0.41524,1.532556,1.99747,2.922224,0.680795,1.135119,-2.716828,0.197128,0.413113,0.12993,0.084041,1.59022,2.080086,-2.969668,-1.549816,-4.662428,-0.952339,-1.066946,2.632252,0.335161,-0.568012,0.797751,0.150585,-2.615431,1.807879,3.520617,2.135913,1.761449,-2.739461,0.499412,-0.721731,-0.625727,0.387143,0.926526,0.080492,0.759661,2.243973,-4.207169,-2.985669,-2.898501,0.000767,2.179774,-1.590896,-0.233298,1.413049,-0.419636,-2.098011,3.362456,4.657073,3.227016,-3.802002,0.438184,-1.729312,-1.345728,0.168235,1.305949,1.78705,-0.012103,2.884615,-2.841476,-3.064535,-4.648148,-1.386558,3.767921,-1.263985,0.498792,0.427309,-1.064913,1.825929,-3.283951,1.504099,4.922847,2.819517,1.925252,-2.828286,-0.147149,-1.929507,-1.372178,0.186208,1.079947,1.797088,0.119033,1.253607,0.84674,-4.362126,-4.225026,-1.30981,0.058191,2.390331,-0.536559,-0.828161,0.76006,1.234235,-2.038607,-0.754647,2.676758,4.293722,1.789626,1.435983,-2.702702,0.155509,-1.728699,-1.74031,-0.931609,0.442194,1.208973,1.724771,0.081559,1.125496,1.361895,-4.396658,-1.827841,-3.143515,-1.395719,1.669301,1.348987,-0.912229,-0.331671,0.273654,-0.278915,0.400967,0.982733,-2.19254,2.439494,1.287185,0.516188,4.659735,-0.075651,-2.72047,-0.724299,-0.014946,0.814853,1.925998,1.131204,-0.199323,-2.753945,-2.063139,-1.54893,-0.428366,-0.162741,-0.354424,-0.377889,-0.136946,0.057104,0.241546,-0.639826,-1.56205,-0.232173,-2.371991,-1.474798,-1.729587,-1.53105,-0.444301,-0.417608,-0.149653,0.412268,0.691785,1.001197,1.794087,2.604812,1.759419,2.109368,2.875391,1.708656];
b_y = [0.150308,0.032733,0.087477,-0.184861,-0.19176,-0.244545,-0.620013,-0.315863,0.044172,0.495311,0.513282,0.098648,-0.319466,-0.022333,0.466706,0.324884,-0.017457,-0.315268,-0.508207,-0.091324,0.253974,-0.18902,1.246099,0.789747,1.235156,2.39015,1.569555,0.248839,-0.442824,-1.831861,-2.215387,-0.404912,-0.627239,-0.710472,0.684659,1.313102,2.274493,1.12604,0.617582,0.072342,-0.602712,-0.831929,-1.811983,-2.484563,-1.238869,-0.024541,1.061533,1.820569,1.64539,1.656407,0.502966,-0.885647,-1.429696,-0.914259,-1.494008,-1.178915,0.081687,0.388722,1.872813,2.140445,0.912576,0.834504,-0.345805,-1.120656,-1.141259,-2.759314,-1.714735,-0.637067,1.270633,1.604954,2.33094,0.848922,0.087644,-0.126196,-0.192313,0.783276,-0.703215,-0.404117,-0.884847,-0.275746,1.068547,2.031577,-0.035815,1.086796,0.736291,-0.356892,-1.294565,-1.3968,-1.463568,-2.948729,-0.641548,0.936264,1.641624,2.214704,0.618803,0.548375,0.006213,-0.364642,-0.406611,-0.6013,-1.620139,-1.680205,-0.530021,1.359571,2.457094,1.066134,2.034931,1.569868,0.826547,-0.86175,-2.108948,-1.737151,-2.944669,-2.073491,0.965144,2.30753,2.660513,1.019833,0.451995,0.045784,-0.90523,0.365214,-2.039151,-1.320157,-0.84062,-0.814682,1.224056,1.657592,0.62847,-0.092896,-0.342768,-0.896352,-2.379559,-1.224859,-1.413725,0.937441,1.074394,3.082921,3.044776,1.575103,0.288731,-0.088987,-0.013328,-0.320904,-1.263598,-3.548985,-2.157171,-0.380233,1.838656,2.849572,0.860727,1.405748,0.837143,-0.420964,-1.439596,-1.409223,-2.397803,-2.454196,-0.841701,0.993403,3.277784,4.109488,1.954403,-0.081059,0.116251,-0.235608,-1.107628,-4.178619,-4.212148,-0.407236,1.325991,3.527796,1.768778,2.752812,1.554212,-0.23964,-1.378379,-2.681856,-2.167458,-3.721117,-1.332264,0.899594,2.123586,5.425502,3.04866,0.367986,0.8177,0.114099,-1.035504,-0.362,-2.166615,-4.703103,-2.272566,-0.600503,2.057575,3.13025,2.158235,2.699796,1.557448,-0.139276,-1.180845,-2.35652,-2.090325,-3.14574,-1.583309,-0.190413,1.795001,3.791829,3.018544,1.463978,0.035855,-0.092358,-0.334621,-0.020697,-0.943398,-3.055418,-3.129357,-1.117009,-0.82679,1.732919,2.81286,1.765433,2.697048,1.947134,1.231765,-0.407003,-1.315,-2.384022,-2.206484,-2.852428,-1.466985,-0.529254,1.117203,1.905909,4.068913,2.083277,0.817743,-0.230035,0.554929,0.396449,-1.033724,-1.145299,0.514937,-0.435344,-2.35955,-2.706204,-1.349019,-1.066459,1.025397,1.195342,0.279197,-0.527818,-0.889185,0.211459,1.38808,2.383104,1.359487,0.775411,0.586908,0.835092,0.293265,-0.211836,-0.136022,-0.370937,-0.781629,-1.285391,-3.204074,0.803108,5.040867,0.118342,-0.085021,0.329522,0.327459,-0.409828,0.052254,-0.064908,-1.096745,-2.481432,-1.307755,-1.600773,-3.462572,-2.017768,-1.205342,-1.0628,-0.434529];
bb = sqrt(b_x.^2 + b_y.^2);

%% testing script Welch
%Welch script does what pwelch or cpsd functuion do
fs = 30;
window = hamming(256);
overlap = 255;

cpsd_script = Welch(a,b,window,overlap, fs);%%
cpsd_matlab = cpsd(a,b,window,overlap,256,fs);

subplot 211; plot(abs(cpsd_matlab))
subplot 212; plot(abs(cpsd_script))
%% welche 
window= hamming(256);
pw = pwelch(a,window,255,256,30);

%% coherence - 2 ways of computation
cohe = mscohere(a,b,window,255,256,30);
cohe2 = cpsd(a,b,window,255,256,30).^2 ./ (pwelch(a,window,255,256,30) .* pwelch(b,window,255,256,30));

subplot 211; plot(abs(cohe))
subplot 212; plot(abs(cohe2))

%% check with dotnet output
%output.m is generated by dotnet interactive, it creates vector dn
close all; plot(abs(cohe));output; hold on; plot(dn)