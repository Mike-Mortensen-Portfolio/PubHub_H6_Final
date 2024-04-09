﻿using PubHub.Common.Models.ContentTypes;

namespace PubHub.Common.Services
{
    public interface IContentTypeService
    {
        Task<ServiceResult<IReadOnlyList<ContentTypeInfoModel>>> GetAllContentTypesAsync();
        [Obsolete($"Use {nameof(GetAllContentTypesAsync)} instead.")]
        Task<IReadOnlyList<ContentTypeInfoModel>> GetContentTypesAsync();
    }
}
