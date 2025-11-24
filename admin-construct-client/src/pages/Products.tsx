import { useState, useEffect } from 'react';
import { productService, type ProductDto } from '../services/productService.tsx';
import { ShoppingCart } from 'lucide-react';
import { notifications } from '../utils/notifications.ts';

export default function Products() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    loadProducts();
  }, [page]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productService.getProducts(page, 10);
      setProducts(data.items);
      setTotalPages(Math.ceil(data.totalCount / data.pageSize));
    } catch (error) {
      console.error('Error loading products:', error);
      notifications.error('Error al cargar los productos');
    } finally {
      setLoading(false);
    }
  };

  const addToCart = (product: ProductDto) => {
    const cart = JSON.parse(localStorage.getItem('cart') || '[]');
    const existingItem = cart.find((item: any) => item.id === product.id && item.type === 'product');
    
    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      cart.push({ ...product, quantity: 1, type: 'product' });
    }
    
    localStorage.setItem('cart', JSON.stringify(cart));
    notifications.success('Producto agregado al carrito');
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-primary mb-8">Catálogo de Productos</h1>

      {loading ? (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {products.map((product) => (
              <div key={product.id} className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition">
                <div className="p-6">
                  <h3 className="text-xl font-semibold text-gray-800 mb-2">{product.name}</h3>
                  {product.description && (
                    <p className="text-gray-600 text-sm mb-3">{product.description}</p>
                  )}
                  <div className="flex justify-between items-center mb-3">
                    <span className="text-sm text-gray-500">Stock: {product.stockQuantity}</span>
                    <span className={`px-2 py-1 rounded text-xs ${product.stockQuantity > 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                      {product.stockQuantity > 0 ? 'Disponible' : 'Agotado'}
                    </span>
                  </div>
                  <p className="text-2xl font-bold text-primary mb-4">
                    ${product.price.toFixed(2)}
                  </p>
                  <button
                    onClick={() => addToCart(product)}
                    disabled={product.stockQuantity === 0}
                    className="w-full bg-secondary text-white py-2 rounded-lg hover:bg-secondary-dark transition flex items-center justify-center space-x-2 disabled:opacity-50 disabled:cursor-not-allowed font-medium"
                  >
                    <ShoppingCart size={20} />
                    <span>Agregar al Carrito</span>
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Paginación */}
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
        </>
      )}
    </div>
  );
}
