Push-Location ..\..\docker
docker build -f indy-pool.dockerfile -t indy_pool .
docker run -itd -p 9701-9709:9701-9709 indy_pool
Pop-Location