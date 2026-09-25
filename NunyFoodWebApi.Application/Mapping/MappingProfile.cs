using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Admins;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.DTOs.Customers;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.DTOs.PackProducts;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<Beneficiary, BeneficiaryDto>();
        CreateMap<Product, ProductDto>();
        CreateMap<Pack, PackDto>();
        CreateMap<PackProduct, PackProductDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<Payment, PaymentDto>();
        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>();
        CreateMap<DeliveryAgent, DeliveryAgentDto>();
        // Les chemins de stockage restent privés : on expose les routes authentifiées qui servent les fichiers.
        CreateMap<Delivery, DeliveryDto>()
            .ForCtorParam(nameof(DeliveryDto.PhotoUrl), o => o.MapFrom(d => d.PhotoPath == null ? null : $"/api/deliveries/{d.Id}/photo"))
            .ForCtorParam(nameof(DeliveryDto.SignatureUrl), o => o.MapFrom(d => d.SignaturePath == null ? null : $"/api/deliveries/{d.Id}/signature"));
        CreateMap<Admin, AdminDto>();
    }
}
