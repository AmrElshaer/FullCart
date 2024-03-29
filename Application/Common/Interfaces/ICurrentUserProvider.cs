﻿using Application.Common.models;

namespace Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    UserDto  GetCurrentUser();
}
