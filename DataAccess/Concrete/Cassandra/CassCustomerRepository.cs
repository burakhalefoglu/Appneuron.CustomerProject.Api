﻿using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassCustomerRepository : CassandraRepositoryBase<Customer>, ICustomerRepository
{
    public CassCustomerRepository() : base(MappingConfiguration.Global.Define<CustomerMapper>())
    {
    }
}