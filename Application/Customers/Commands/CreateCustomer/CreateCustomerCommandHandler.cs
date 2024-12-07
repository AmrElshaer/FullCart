using Domain.Users;

namespace Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Guid>>
{
    private readonly ICartDbContext _dbContext;
    private readonly IIdentityService _identityService;

    public CreateCustomerCommandHandler(ICartDbContext dbContext, IIdentityService identityService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        if (email.IsError) return email.Errors;

        var address = Address.Create(request.City, request.Street, request.State);

        if (address.IsError) return address.Errors;
        var user = await _identityService.CreateUserAsync(email.Value,
            UserType.Customer,
            request.Password,
            Roles.Customer);

        if (user.IsError) return user.Errors;
        var customer = new Customer(user.Value.Id, address.Value);
       // await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}