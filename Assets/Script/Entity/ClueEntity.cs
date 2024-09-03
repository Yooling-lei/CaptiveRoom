namespace Script.Entity
{
    public class ClueEntity
    {
        public int ID { get; set; }
        
        // 标题
        public string Title { get; set; }
        
        // 详情
        public string Detail { get; set; }
        
        // 分类
        public string Category { get; set; }
        
        // 目录
        public string Catalog { get; set; }
        
        // 关联的图片信息
        public string ImagePath { get; set; }
    }
}