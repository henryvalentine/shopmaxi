using AutoMapper;

namespace IShopkeeperServices.ModelMapper
{
    public static class ModelCrossMapper
    {
        public static TModel Map<TObject, TModel>(TObject objectModel)
        {
            return Mapper.DynamicMap<TObject, TModel>(objectModel);
        }
        
    }
    
}
