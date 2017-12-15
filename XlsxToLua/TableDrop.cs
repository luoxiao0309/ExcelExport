using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Table_drop
{
    public  string id = "id";
    public  string prob = "prob";
    public string drop_id = "drop_id";
    public  string min_lv = "min_lv";
    public  string max_lv = "max_lv";
    public  string drop_group = "drop_group";

    public string name { get; set; }

    private Dictionary<int, List<Table_dropField>> _Drop_ID = new Dictionary<int, List<Table_dropField>>();
    /// <summary>
    /// 以掉落ID为key，生成字典
    /// </summary>
    public Dictionary<int,List<Table_dropField>> Drop_ID { get; set; }
    /// <summary>
    /// 储存所有掉落的id。以掉落ID为key，生成字典
    /// </summary>
    /// <param name="dropid"></param>
    /// <param name="dropfield"></param>
    public void AddDropID(int dropid,int probability, Table_dropField  dropfield)
    {
        if (_Drop_ID.ContainsKey(dropid))
            _Drop_ID[dropid].Add(dropfield);
        else
        _Drop_ID.Add(dropid, new List<Table_dropField> { dropfield });

        if(probability==10000)
        {
            if (_Drop_ID100.ContainsKey(dropid))
                _Drop_ID100[dropid].Add(dropfield);
            else
            _Drop_ID100.Add(dropid, new List<Table_dropField> { dropfield });
        }
        else if (probability == 0)
        {
            if (_Drop_ID0.ContainsKey(dropid))
                _Drop_ID0[dropid].Add(dropfield);
            else
            _Drop_ID0.Add(dropid, new List<Table_dropField> { dropfield });
        }
        else
        {
            if (_Drop_ID50.ContainsKey(dropid))
                _Drop_ID50[dropid].Add(dropfield);
            else
            _Drop_ID50.Add(dropid, new List<Table_dropField> { dropfield });
        }
    }

    private Dictionary<int, List<Table_dropField>> _Drop_ID100 = new Dictionary<int, List<Table_dropField>>();
    /// <summary>
    /// 储存必定掉落的。以掉落ID为key，生成字典
    /// </summary>
    public Dictionary<int, List<Table_dropField>> Drop_ID100 { get; set; }
    private Dictionary<int, List<Table_dropField>> _Drop_ID50 = new Dictionary<int, List<Table_dropField>>();
    /// <summary>
    ///  储存有概率（概率不为0也不为10000的）掉落的。以掉落ID为key，生成字典
    /// </summary>
    public Dictionary<int, List<Table_dropField>> Drop_ID50 { get; set; }
    /// <summary>
    /// 存有必定【不】掉落的。以掉落ID为key，生成字典
    /// </summary>
    private Dictionary<int, List<Table_dropField>> _Drop_ID0 = new Dictionary<int, List<Table_dropField>>();
    public Dictionary<int, List<Table_dropField>> Drop_ID0 { get; set; }

}
public class Table_dropField
{
    public int row { get; set; }
    public int id { get; set; }
    /// <summary>
    /// 概率
    /// </summary>
    public int prob { get; set; }
    public int min_lv { get; set; }
    public int max_lv { get; set; }
    public int drop_group { get; set; }
}
public class Table_drop_reward
{
    public string id = "id";
    public string drop_group = "drop_group";
    public string prob = "prob";
    public string itemtype = "type";
    public string item_id = "item_id";
    public string num = "num";


    public string name { get; set; }

    private Dictionary<int, List<table_drop_rewardField>> _Drop_Group = new Dictionary<int, List<table_drop_rewardField>>();
    /// <summary>
    /// 储存所有掉落的ID，以drop_group为key
    /// </summary>
    public Dictionary<int,List<table_drop_rewardField>> Drop_Group { get; set; }
    private Dictionary<int, List<table_drop_rewardField>> _Drop_Group100 = new Dictionary<int, List<table_drop_rewardField>>();
    /// <summary>
    /// 储存所有掉落的ID，以drop_group为key
    /// </summary>
    public Dictionary<int, List<table_drop_rewardField>> Drop_Group100 { get; set; }
    private Dictionary<int, List<table_drop_rewardField>> _Drop_Group50 = new Dictionary<int, List<table_drop_rewardField>>();
    /// <summary>
    /// 储存所有掉落的ID，以drop_group为key
    /// </summary>
    public Dictionary<int, List<table_drop_rewardField>> Drop_Group50 { get; set; }
    private Dictionary<int, List<table_drop_rewardField>> _Drop_Group0 = new Dictionary<int, List<table_drop_rewardField>>();
    /// <summary>
    /// 储存所有掉落的ID，以drop_group为key
    /// </summary>
    public Dictionary<int, List<table_drop_rewardField>> Drop_Group0 { get; set; }

    public void AddDropGroup(int dropgroup,table_drop_rewardField droupRewarField)
    {
        if (_Drop_Group.ContainsKey(dropgroup))
        {
            _Drop_Group[dropgroup].Add(droupRewarField);
        }
        else
        {
            _Drop_Group.Add(dropgroup, new List<table_drop_rewardField> { droupRewarField });
        }
        
    }

    public void GetDropGroupRandom()
    {
        foreach(KeyValuePair<int,List<table_drop_rewardField>> kvp in _Drop_Group)
        {
            int sum_prop = 0;
            foreach(table_drop_rewardField tdr in kvp.Value)
            {
                sum_prop = sum_prop + tdr.prob;

                switch(tdr.itemtype)
                {
                    case (int)ItemType.伙伴:
                        break;

                    case (int)ItemType.装备:
                        break;

                    case (int)ItemType.宝物:
                        break;

                    case (int)ItemType.道具:

                        break;

                    default:
                        break;

                }
            }


            if(sum_prop>0)
            {
                if(kvp.Value.Count>1)
                _Drop_Group50.Add(kvp.Key, kvp.Value);

                _Drop_Group100.Add(kvp.Key, kvp.Value);
            }
            else
            {
                _Drop_Group0.Add(kvp.Key, kvp.Value);
            }
            

        }
    }
}

public class table_drop_rewardField
{
    public int row { get; set; }
    public int id { get; set; }
    /// <summary>
    /// 权重
    /// </summary>
    public int prob { get; set; }
    /// <summary>
    /// 掉落道具的类型
    /// </summary>
    public int itemtype { get; set; }
    /// <summary>
    /// 掉落道具ID
    /// </summary>
    public int item_id { get; set; }
    /// <summary>
    /// 掉落道具数量
    /// </summary>
    public int num { get; set; }

}
public enum ItemType
{
    元宝=1,
    铜钱=2,
    木材=3,
    灵石=4,
    友情值=5,
    寒铁精华=6,
    货币币=7,
    仙盟个人贡献=8,
    亲密度=9,
    甜蜜值=10,
    荣誉=11,
    宝物精华=12,
    药草精华=13,
    角色经验=14,
    VIP值=15,

    伙伴=21,
    装备=31,
    宝物=32,
    道具=41

}
public class DropTableAnalyzeHelper
{
    public static void AnalyzeDrop(TableInfo table, out string errorString)
    {
        errorString = null;
        Table_drop table_drop = new Table_drop();

        table_drop.name = table.TableName;
        //数据表的行数
        int rowCount = table.GetKeyColumnFieldInfo().Data.Count;

        StringBuilder errorStringBuild = new StringBuilder();
        for (int row = 0; row < rowCount; ++row)
        {
            Table_dropField table_dropfield = new Table_dropField();

            table_dropfield.row = row + 5;
            table_dropfield.id = (int)table.GetFieldInfoByFieldName(table_drop.id).Data[row];
            table_dropfield.prob = (int)table.GetFieldInfoByFieldName(table_drop.prob).Data[row];
            table_dropfield.min_lv = (int)table.GetFieldInfoByFieldName(table_drop.min_lv).Data[row];
            table_dropfield.max_lv = (int)table.GetFieldInfoByFieldName(table_drop.max_lv).Data[row];
            table_dropfield.drop_group = (int)table.GetFieldInfoByFieldName(table_drop.drop_group).Data[row];

            int dropid = (int)table.GetFieldInfoByFieldName(table_drop.drop_id).Data[row];
            int probability = (int)table.GetFieldInfoByFieldName(table_drop.prob).Data[row];
            table_drop.AddDropID(dropid, probability, table_dropfield);

            //bool b = AppValues.TableDropRewardInfo[table_drop.name + "_reward"].Drop_Group.ContainsKey(table_dropfield.drop_group);
            bool b = false;
            try
            {
                b = AppValues.TableInfo[table_drop.name + "_reward"].GetFieldInfoByFieldName("drop_group").Data.Contains(table_dropfield.drop_group);
            }
            catch { }
            if (b == false)
            {
                errorStringBuild.AppendFormat("第{0}行{1}列数据：{2}错误，未在{3}表中{4}字段找到对应值\n", row + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(table.GetFieldInfoByFieldName("drop_group").ColumnSeq + 1), table_dropfield.drop_group, table.TableName + "_reward", "drop_group");
            }
        }
        if (errorStringBuild.Length > 0)
            errorString = errorStringBuild.ToString();
        AppValues.TableDropInfo.Add(table_drop.name, table_drop);
    }
    public static void AnalyzeDroupGroup(TableInfo table,out string errorString)
    {
        errorString = null;
        Table_drop_reward table_drop_reward = new Table_drop_reward();

        table_drop_reward.name = table.TableName;

        string table_drop = table.TableName.Substring(0, table.TableName.Length - 7);
       //数据表的行数
       int rowCount = table.GetKeyColumnFieldInfo().Data.Count;

        StringBuilder errorStringBuild = new StringBuilder();
        for (int row = 0; row < rowCount; ++row)
        {
            table_drop_rewardField table_drop_rewardfield = new table_drop_rewardField();

            int itemtype= (int)table.GetFieldInfoByFieldName(table_drop_reward.itemtype).Data[row];
            object item_id = table.GetFieldInfoByFieldName(table_drop_reward.item_id).Data[row];
            int num= (int)table.GetFieldInfoByFieldName(table_drop_reward.num).Data[row];

            table_drop_rewardfield.row = row + AppValues.DATA_FIELD_DATA_START_INDEX + 1;
            table_drop_rewardfield.id = (int)table.GetFieldInfoByFieldName(table_drop_reward.id).Data[row];
            table_drop_rewardfield.prob = (int)table.GetFieldInfoByFieldName(table_drop_reward.prob).Data[row];
            table_drop_rewardfield.item_id = (int)item_id;
            table_drop_rewardfield.itemtype = itemtype;
            table_drop_rewardfield.num = num;


            object dropid = table.GetFieldInfoByFieldName(table_drop_reward.drop_group).Data[row];
            table_drop_reward.AddDropGroup((int)dropid, table_drop_rewardfield);

            if(!GetItemNameByID(itemtype, item_id))
            {
                errorStringBuild.AppendFormat("第{0}行数据错误，道具类型与ID不匹配，道具类型为{1}，道具ID为{2}\n", row + AppValues.DATA_FIELD_DATA_START_INDEX + 1,itemtype,(int)item_id);
            }
           
          bool b=  AppValues.TableInfo[table_drop].GetFieldInfoByFieldName("drop_group").Data.Contains(dropid);
            if(b==false)
            {
                errorStringBuild.AppendFormat("第{0}行{1}列数据：{2}错误，未在{3}表中{4}字段找到对应值\n", row + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(table.GetFieldInfoByFieldName(table_drop_reward.drop_group).ColumnSeq + 1), dropid,table_drop, "drop_group");
            }

            if(num <=0)
            {
                errorStringBuild.AppendFormat("第{0}行{1}列数据不符合要求，应该大于零，填入值为：{2}\n", row + AppValues.DATA_FIELD_DATA_START_INDEX + 1, Utils.GetExcelColumnName(table.GetFieldInfoByFieldName(table_drop_reward.drop_group).ColumnSeq + 1));
            }
        }
        if(errorStringBuild.Length !=0)
            errorString = errorStringBuild.ToString();

        AppValues.TableDropRewardInfo.Add(table_drop_reward.name, table_drop_reward);
    }

    public static bool GetItemNameByID(int itemType, object ID)
    {
        string entry_item = "entry_item";
        string entry_item_field = "item_id";

        string entry_currency = "entry_currency";
        string entry_currency_field = "item_id";

        string entry_item_weapon = "entry_item_weapon";
        string entry_item_weapon_field = "weapon_id";

        string entry_partner = "entry_partner";
        string entry_partner_field = "entry_id";

        string entry_baowu = "entry_baowu";
        string entry_baowu_field = "id";

        switch (itemType)
        {
            case (int)ItemType.伙伴:
                return AppValues.TableInfo[entry_partner].GetFieldInfoByFieldName(entry_partner_field).Data.Contains(ID);

            case (int)ItemType.装备:
                return AppValues.TableInfo[entry_item_weapon].GetFieldInfoByFieldName(entry_item_weapon_field).Data.Contains(ID);

            case (int)ItemType.宝物:
                return AppValues.TableInfo[entry_baowu].GetFieldInfoByFieldName(entry_baowu_field).Data.Contains(ID);

            case (int)ItemType.道具:
                return AppValues.TableInfo[entry_item].GetFieldInfoByFieldName(entry_item_field).Data.Contains(ID);

            default:
                return AppValues.TableInfo[entry_currency].GetFieldInfoByFieldName(entry_currency_field).Data.Contains(itemType);

        }
    }
}

