-- systemId                         int                              系统ID
-- systemName                       lang                             系统名称
-- help                             lang                             系统帮助信息
-- openCondition                    dict[3]                          开启条件（同时满足三个条件）
--    rankLimit                     int                              所需玩家等级（不限填-1）
--    vipLimit                      int                              所需Vip等级（不限填-1）
--    levelLimit                    int                              所需通关关卡（不限填-1）
-- openRewards                      array[dict[3]:2]                 开启系统后赠送物
--    [1]                           dict[3]                          奖励物1
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量
--    [2]                           dict[3]                          奖励物2
--       type                       int                              类型
--       id                         int                              id
--       count                      int                              数量

return {
	[1] = {
		systemName = "",
		help = "",
		openCondition = nil,
		openRewards = nil,
	},
	[2] = {
		systemName = "",
		help = "",
		openCondition = nil,
		openRewards = nil,
	},
	[3] = {
		systemName = "",
		help = "",
		openCondition = {
			rankLimit = 25,
			vipLimit = -1,
			levelLimit = -1,
		},
		openRewards = {
			[1] = {
				type = 1,
				id = 100001,
				count = 10,
			},
			[2] = nil,
		},
	},
	[4] = {
		systemName = "",
		help = "",
		openCondition = {
			rankLimit = 15,
			vipLimit = -1,
			levelLimit = -1,
		},
		openRewards = {
			[1] = {
				type = 1,
				id = 100001,
				count = 5,
			},
			[2] = nil,
		},
	},
}
