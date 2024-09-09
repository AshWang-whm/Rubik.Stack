1.是否考虑后台管理&功能系统分开2个项目/2个数据库？管理系统可对数据库，程序加密
2.

低代码平台架构设计
	1.Orm：Freesql，结合Freesql.ZeroEntity，实现自定义表结构crud
	2.可拖拽布局组件- blazor-dragdrop
		2.1.可拖拽布局：分左中右3块面板
			左：可选控件：文本框，数值框，下拉框，单选框，复选框，TextArea，FileUpload
			中：渲染已选组件，Row组件为容器
			右：组件样式设置
		2.2.Yitter.IdGenerator:生成唯一主键
	3.Service
		3.2.FileUpload，文件上传相关,三种方案：1.保存到配置文件的路径，2.对接FileServer，3.对接minio等组件
		3.3.FormValidation，表单提交验证，FluentValidation组件
		3.4.SetupForm，创建表单
		3.5.SetupPage，创建页面
	4.Data
		4.1.EntityHandler ，生成数据库表操作，实体转freesql ZeroEntity json等
		4.2.数据表管理，目前只提供基础类型数据
		4.3.数据源管理
			1.本地数据源：配置ZeroEntity查询语句，参数，先支持where 条件的配置，暂不考虑 left join 中的where条件
			2.API数据源：配置api地址，查询参数，报文json
		4.4.动态接口管理
	5.配置管理
		1.组件配置：UniqueID，配置名称，类型(页面，表单)
		2.组件界面配置：组件ID，界面渲染json（布局，容器控件，绑定的数据源字段，按钮功能）
			2.1.按钮：
				1.查询按钮：数据源ID，控件绑定字段与数据源字段对应配置
				2.跳转按钮：配置页面url，参数等
				3.弹框按钮：Modal打开新的组件，当前数据源字段，生成对应新组件的参数，最后Modal.Show()
				4.提交按钮：仅支持表单的提交，数据验证，通过ZeroEntity新增/更新数据，复杂的页面操作通过api提交。接口协议默认使用HttpContract，仅支持简单验证success&false，false时弹框输出msg，考虑保存api返回结果，暂不考虑自定义json。
				5.删除按钮：仅支持表格批量删除，表格行内删除功能
				*.通用功能：完成操作后success提示，false提示，刷新表格
			2.2.表格：
				1.表格名称
				2.表格数据源
				3.显示字段
				4.行内按钮（编辑，删除，其他按钮）
				5.明细行组件（表格/其他组件）？
				6.表头合并？
			2.3.表单：需要先选择数据源，设置初始化参数。
				*.通用配置：控件名称，绑定字段
				1.Row配置，默认从上往下，逐行配置
				2.
			2.4.JS交互
			2.5.展示组件
				1.Tag
				2.Progress
				3.List
				4.
	6.执行流程
		1.主页：获取用户权限，获取用户菜单数据，生成菜单
		2.打开页面：获取布局配置json & 各组件数据源配置json > 对碰用户权限&生成页面 > 控件绑定字段&值 > 加载数据
		3.页面按钮：
			3.1.查询按钮：触发查询数据源配置json 》 页面其他控件的值&数据源查询条件做对应绑定，生成ZeroEntity查询json 》返回数据集绑定数据源
			3.2.弹框按钮：Modal.Show\<DyanmicComponent>\() , DyanmicComponent 通过组件ID获取页面配置并渲染组件，如有数据参数，查询数据并渲染值
			3.3.确认按钮：仅支持表单类控件使用。通过表单json&控件值&验证条件，执行表单验证 》验证通过后通过表单json&控件值，生成ZeroEntity json》执行Insert Or Update
			3.4.删除按钮：仅支持表格控件使用，选中表格项/点击表格行内删除按钮，绑定数据ID 》 通过表格配置json&所选数据ID，生成ZeroEntity json 》 执行 Update ，统一使用软删除
		4.表格：通过数据源配置json 》 获取数据 》 返回json数据 》 生成表格&表头控件&行内控件
		5.表单：通过表单配置json，生成页面 》 如有数据参数&初始化数据源json，执行查询并初始化表单 》 保存表单 》 通过验证json执行表单验证 》 表单数据源json&表单数据，生成ZeroEntity json》执行Insert Or Update





