using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Linq;

namespace DatingApp.API.Helpers
{
    public class DatingProfile : Profile
    {
        public DatingProfile()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(des => des.PhotoUrl,
                    opts => opts.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(des => des.Age,
                    opts => opts.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailsDto>()
                .ForMember(des => des.PhotoUrl,
                    opts => opts.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(des => des.Age,
                    opts => opts.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<RegisterDto, User>();
            CreateMap<Photo, PhotoForDetailsDto>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<PhotoForUploadDto, Photo>();
            CreateMap<Photo, PhotoForListDto>();

            CreateMap<CreateMessageDto, Message>();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(des => des.SenderPhotoUrl, opts =>
                {
                    opts.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(des => des.RecipientPhotoUrl, opts =>
                {
                    opts.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ReverseMap();
        }
    }
}
