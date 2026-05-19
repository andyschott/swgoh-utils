container image pull postgres:18
container image pull adminer

container run --name postgres \
  --detach --rm \
  --env POSTGRES_USER=admin \
  --env POSTGRES_PASSWORD=password \
  --env POSTGRES_DB=swgoh \
  --volume swgoh_pgdata:/var/lib/postgresql/ \
  postgres:18

container run --name adminer \
  --detach --rm \
  adminer
