低代码平台架构设计
	1.Orm：Freesql，结合Freesql.ZeroEntity，实现自定义表结构crud
	2.可拖拽布局组件- blazor-dragdrop
		2.1.可拖拽布局：分左中右3块面板
			左：可选控件：文本框，数值框，下拉框，单选框，复选框，TextArea，FileUpload
			中：渲染已选组件，Row组件为容器
			右：组件样式设置
		2.2.Yitter.IdGenerator:生成唯一主键
	3.Service
		3.1.EntityHandler ，生成数据库表操作，实体转freesql ZeroEntity json等
		3.2.FileUpload，文件上传相关,三种方案：1.保存到配置文件的路径，2.对接FileServer，3.对接minio等组件
		3.3.FormValidation，表单提交验证，FluentValidation组件
		3.4.SetupForm，创建表单
		3.5.SetupPage，创建页面
