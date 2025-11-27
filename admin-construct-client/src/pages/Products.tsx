import { useState, useEffect } from 'react';
import { productService, type ProductDto } from '../services/productService.ts';
import { ShoppingCart, Search, Filter } from 'lucide-react';
import { notifications } from '../utils/notifications.ts';
import { useCart } from '../context/CartContext';

export default function Products() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const { addToCart } = useCart();


  useEffect(() => {
    loadProducts();
  }, [page]);

  useEffect(() => {
    if (searchTerm.trim() === '') {
      setFilteredProducts(products);
    } else {
      const lowerTerm = searchTerm.toLowerCase();
      const filtered = products.filter(product => 
        product.name.toLowerCase().includes(lowerTerm) || 
        (product.description && product.description.toLowerCase().includes(lowerTerm))
      );
      setFilteredProducts(filtered);
    }
  }, [searchTerm, products]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productService.getProducts(page, 100); // Cargar más para filtrado cliente-side efectivo
      setProducts(data.items);
      setFilteredProducts(data.items);
      setTotalPages(Math.ceil(data.totalCount / data.pageSize));
    } catch (error) {
      console.error('Error loading products:', error);
      notifications.error('Error al cargar los productos');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = (product: ProductDto) => {
    addToCart({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1,
      type: 'product'
    });
    notifications.success('Producto agregado al carrito');
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex flex-col md:flex-row justify-between items-center mb-8 gap-4">
        <h1 className="text-3xl font-bold text-primary">Catálogo de Productos</h1>
        
        <div className="relative w-full md:w-96">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Search size={20} className="text-gray-400" />
          </div>
          <input
            type="text"
            placeholder="Buscar productos..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none transition shadow-sm"
          />
        </div>
      </div>

      {loading ? (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      ) : (
        <>
          {filteredProducts.length === 0 ? (
            <div className="text-center py-12 bg-gray-50 rounded-lg">
              <Filter size={48} className="mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500 text-lg">No se encontraron productos que coincidan con tu búsqueda.</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filteredProducts.map((product) => (
                <div key={product.id} className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition flex flex-col h-full">
                  <div className="relative h-48 bg-gray-200 overflow-hidden group">
                    {product.imageUrl ? (
                      <img 
                        src={product.imageUrl} 
                        alt={product.name} 
                        className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center bg-gray-100 text-gray-400">
                        <span className="text-4xl font-bold opacity-20">IMG</span>
                      </div>
                    )}
                    {product.stockQuantity === 0 && (
                      <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                        <span className="bg-red-600 text-white px-3 py-1 rounded-full font-bold transform -rotate-12">AGOTADO</span>
                      </div>
                    )}
                  </div>
                  
                  <div className="p-6 flex-grow flex flex-col">
                    <h3 className="text-xl font-semibold text-gray-800 mb-2 line-clamp-1" title={product.name}>{product.name}</h3>
                    {product.description && (
                      <p className="text-gray-600 text-sm mb-3 line-clamp-2 flex-grow" title={product.description}>{product.description}</p>
                    )}
                    
                    <div className="mt-auto">
                      <div className="flex justify-between items-center mb-3">
                        <span className="text-sm text-gray-500">Stock: {product.stockQuantity}</span>
                        <span className={`px-2 py-1 rounded text-xs font-medium ${product.stockQuantity > 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                          {product.stockQuantity > 0 ? 'Disponible' : 'Agotado'}
                        </span>
                      </div>
                      <p className="text-2xl font-bold text-primary mb-4">
                        ${product.price.toFixed(2)}
                      </p>
                      <button
                        onClick={() => handleAddToCart(product)}
                        disabled={product.stockQuantity === 0}
                        className="w-full bg-secondary text-white py-2 rounded-lg hover:bg-secondary-dark transition flex items-center justify-center space-x-2 disabled:opacity-50 disabled:cursor-not-allowed font-medium active:scale-95 transform duration-100"
                      >
                        <ShoppingCart size={20} />
                        <span>Agregar al Carrito</span>
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Paginación (solo si no hay búsqueda activa) */}
          {searchTerm === '' && (
            <div className="flex justify-center items-center space-x-4 mt-8">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-4 py-2 bg-primary text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-blue-900 transition"
              >
                Anterior
              </button>
              <span className="text-gray-700">
                Página {page} de {totalPages}
              </span>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="px-4 py-2 bg-primary text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-blue-900 transition"
              >
                Siguiente
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
